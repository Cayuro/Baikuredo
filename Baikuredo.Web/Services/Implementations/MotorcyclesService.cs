using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Motorcycles;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class MotorcyclesService : IMotorcyclesService
    {
        private readonly DataContext _context;

        public MotorcyclesService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<PaginationResponse<MotorcycleDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<Motorcycle> query = _context.Motorcycles.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(m => m.Plate.ToLower().Contains(filter)
                                          || m.Color.ToLower().Contains(filter)
                                          || m.MotorcycleModel!.Name.ToLower().Contains(filter)
                                          || m.MotorcycleModel!.Brand!.Name.ToLower().Contains(filter)
                                          || m.Owner!.FirstName.ToLower().Contains(filter)
                                          || m.Owner!.LastName.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderBy(m => m.Plate)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(m => new MotorcycleDTO
                    {
                        Id = m.Id,
                        Plate = m.Plate,
                        Year = m.Year,
                        Displacement = m.Displacement,
                        Color = m.Color,
                        BrandName = m.MotorcycleModel!.Brand!.Name,
                        ModelName = m.MotorcycleModel!.Name,
                        CategoryName = m.Category!.Name,
                        OwnerName = m.Owner!.FirstName + " " + m.Owner!.LastName
                    })
                    .ToListAsync();

                var pagedList = new PagedList<MotorcycleDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<MotorcycleDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<MotorcycleDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<MotorcycleDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateMotorcycleDTO>> GetOneAsync(Guid id)
        {
            try
            {
                Motorcycle? motorcycle = await _context.Motorcycles.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (motorcycle is null)
                {
                    return Response<UpdateMotorcycleDTO>.Failure($"No se encontró la moto con id: {id}");
                }

                var dto = new UpdateMotorcycleDTO
                {
                    Id = motorcycle.Id,
                    Plate = motorcycle.Plate,
                    Year = motorcycle.Year,
                    Displacement = motorcycle.Displacement,
                    Color = motorcycle.Color,
                    MotorcycleModelId = motorcycle.MotorcycleModelId,
                    CategoryId = motorcycle.CategoryId,
                    OwnerId = motorcycle.OwnerId
                };

                return Response<UpdateMotorcycleDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<UpdateMotorcycleDTO>.Failure(ex);
            }
        }

        public async Task<Response<MotorcycleDTO>> GetDetailAsync(Guid id)
        {
            try
            {
                MotorcycleDTO? dto = await _context.Motorcycles.AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new MotorcycleDTO
                    {
                        Id = m.Id,
                        Plate = m.Plate,
                        Year = m.Year,
                        Displacement = m.Displacement,
                        Color = m.Color,
                        BrandName = m.MotorcycleModel!.Brand!.Name,
                        ModelName = m.MotorcycleModel!.Name,
                        CategoryName = m.Category!.Name,
                        OwnerName = m.Owner!.FirstName + " " + m.Owner!.LastName
                    })
                    .FirstOrDefaultAsync();

                if (dto is null)
                {
                    return Response<MotorcycleDTO>.Failure($"No se encontró la moto con id: {id}");
                }

                return Response<MotorcycleDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<MotorcycleDTO>.Failure(ex);
            }
        }

        public async Task<Response<MotorcycleDTO>> CreateAsync(CreateMotorcycleDTO dto)
        {
            try
            {
                string plate = dto.Plate.Trim().ToUpper();

                bool exists = await _context.Motorcycles.AnyAsync(m => m.Plate == plate);
                if (exists)
                {
                    return Response<MotorcycleDTO>.Failure($"Ya existe una moto con la placa {plate}.");
                }

                string? relationError = await ValidateRelationsAsync(dto.MotorcycleModelId!.Value, dto.CategoryId!.Value, dto.OwnerId!.Value);
                if (relationError is not null)
                {
                    return Response<MotorcycleDTO>.Failure(relationError);
                }

                var entity = new Motorcycle
                {
                    Id = Guid.CreateVersion7(),
                    Plate = plate,
                    Year = dto.Year!.Value,
                    Displacement = dto.Displacement!.Value,
                    Color = dto.Color.Trim(),
                    MotorcycleModelId = dto.MotorcycleModelId!.Value,
                    CategoryId = dto.CategoryId!.Value,
                    OwnerId = dto.OwnerId!.Value
                };

                await _context.Motorcycles.AddAsync(entity);
                await _context.SaveChangesAsync();

                return Response<MotorcycleDTO>.Success("Moto registrada exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<MotorcycleDTO>.Failure(ex);
            }
        }

        public async Task<Response<MotorcycleDTO>> UpdateAsync(UpdateMotorcycleDTO dto)
        {
            try
            {
                Motorcycle? motorcycle = await _context.Motorcycles.FirstOrDefaultAsync(m => m.Id == dto.Id);
                if (motorcycle is null)
                {
                    return Response<MotorcycleDTO>.Failure("La moto no existe.");
                }

                string plate = dto.Plate.Trim().ToUpper();

                bool duplicateExists = await _context.Motorcycles.AnyAsync(m => m.Plate == plate && m.Id != dto.Id);
                if (duplicateExists)
                {
                    return Response<MotorcycleDTO>.Failure("Ya existe otra moto con esa placa.");
                }

                string? relationError = await ValidateRelationsAsync(dto.MotorcycleModelId!.Value, dto.CategoryId!.Value, dto.OwnerId!.Value);
                if (relationError is not null)
                {
                    return Response<MotorcycleDTO>.Failure(relationError);
                }

                motorcycle.Plate = plate;
                motorcycle.Year = dto.Year!.Value;
                motorcycle.Displacement = dto.Displacement!.Value;
                motorcycle.Color = dto.Color.Trim();
                motorcycle.MotorcycleModelId = dto.MotorcycleModelId!.Value;
                motorcycle.CategoryId = dto.CategoryId!.Value;
                motorcycle.OwnerId = dto.OwnerId!.Value;

                _context.Motorcycles.Update(motorcycle);
                await _context.SaveChangesAsync();

                return Response<MotorcycleDTO>.Success("Moto actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<MotorcycleDTO>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                Motorcycle? motorcycle = await _context.Motorcycles.FirstOrDefaultAsync(m => m.Id == id);
                if (motorcycle is null)
                {
                    return Response<object>.Failure("Moto no encontrada.");
                }

                _context.Motorcycles.Remove(motorcycle);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Moto eliminada con éxito.");
            }
            catch (DbUpdateException)
            {
                return Response<object>.Failure("No se puede eliminar la moto porque tiene publicaciones.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        // Revisa que el modelo, la categoría y el propietario existan
        private async Task<string?> ValidateRelationsAsync(Guid motorcycleModelId, Guid categoryId, Guid ownerId)
        {
            if (!await _context.MotorcycleModels.AnyAsync(m => m.Id == motorcycleModelId))
            {
                return "El modelo seleccionado no existe.";
            }

            if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
            {
                return "La categoría seleccionada no existe.";
            }

            if (!await _context.Owners.AnyAsync(o => o.Id == ownerId))
            {
                return "El propietario seleccionado no existe.";
            }

            return null;
        }
    }
}
