using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.MotorcycleModels;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class MotorcycleModelsService : IMotorcycleModelsService
    {
        private readonly DataContext _context;

        public MotorcycleModelsService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<PaginationResponse<MotorcycleModelDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<MotorcycleModel> query = _context.MotorcycleModels.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(m => m.Name.ToLower().Contains(filter) || m.Brand!.Name.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderBy(m => m.Brand!.Name)
                    .ThenBy(m => m.Name)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(m => new MotorcycleModelDTO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        BrandId = m.BrandId,
                        BrandName = m.Brand!.Name,
                        IsActive = m.IsActive
                    })
                    .ToListAsync();

                var pagedList = new PagedList<MotorcycleModelDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<MotorcycleModelDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<MotorcycleModelDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<MotorcycleModelDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateMotorcycleModelDTO>> GetOneAsync(Guid id)
        {
            try
            {
                MotorcycleModel? model = await _context.MotorcycleModels.AsNoTracking()
                                                                        .Include(m => m.Brand)
                                                                        .FirstOrDefaultAsync(m => m.Id == id);

                if (model is null)
                {
                    return Response<UpdateMotorcycleModelDTO>.Failure($"No se encontró el modelo con id: {id}");
                }

                var dto = new UpdateMotorcycleModelDTO
                {
                    Id = model.Id,
                    Name = model.Name,
                    BrandId = model.BrandId,
                    BrandName = model.Brand?.Name,
                    IsActive = model.IsActive
                };

                return Response<UpdateMotorcycleModelDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<UpdateMotorcycleModelDTO>.Failure(ex);
            }
        }

        public async Task<Response<MotorcycleModelDTO>> CreateAsync(CreateMotorcycleModelDTO dto)
        {
            try
            {
                string name = dto.Name.Trim();
                Guid brandId = dto.BrandId!.Value;

                Brand? brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == brandId);
                if (brand is null)
                {
                    return Response<MotorcycleModelDTO>.Failure("La marca seleccionada no existe.");
                }

                bool exists = await _context.MotorcycleModels.AnyAsync(m => m.BrandId == brandId && m.Name.ToLower() == name.ToLower());
                if (exists)
                {
                    return Response<MotorcycleModelDTO>.Failure($"Ya existe el modelo {name} para la marca {brand.Name}.");
                }

                var entity = new MotorcycleModel
                {
                    Id = Guid.CreateVersion7(),
                    Name = name,
                    BrandId = brandId,
                    IsActive = true
                };

                await _context.MotorcycleModels.AddAsync(entity);
                await _context.SaveChangesAsync();

                var resultDto = new MotorcycleModelDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    BrandId = entity.BrandId,
                    BrandName = brand.Name,
                    IsActive = entity.IsActive
                };

                return Response<MotorcycleModelDTO>.Success(resultDto, "Modelo creado exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<MotorcycleModelDTO>.Failure(ex);
            }
        }

        public async Task<Response<MotorcycleModelDTO>> UpdateAsync(UpdateMotorcycleModelDTO dto)
        {
            try
            {
                MotorcycleModel? model = await _context.MotorcycleModels.FirstOrDefaultAsync(m => m.Id == dto.Id);
                if (model is null)
                {
                    return Response<MotorcycleModelDTO>.Failure("El modelo no existe.");
                }

                string name = dto.Name.Trim();
                Guid brandId = dto.BrandId!.Value;

                bool brandExists = await _context.Brands.AnyAsync(b => b.Id == brandId);
                if (!brandExists)
                {
                    return Response<MotorcycleModelDTO>.Failure("La marca seleccionada no existe.");
                }

                bool duplicateExists = await _context.MotorcycleModels
                    .AnyAsync(m => m.BrandId == brandId && m.Name.ToLower() == name.ToLower() && m.Id != dto.Id);
                if (duplicateExists)
                {
                    return Response<MotorcycleModelDTO>.Failure("Ya existe otro modelo con ese nombre para esa marca.");
                }

                model.Name = name;
                model.BrandId = brandId;
                model.IsActive = dto.IsActive;

                _context.MotorcycleModels.Update(model);
                await _context.SaveChangesAsync();

                return Response<MotorcycleModelDTO>.Success("Modelo actualizado con éxito.");
            }
            catch (Exception ex)
            {
                return Response<MotorcycleModelDTO>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                MotorcycleModel? model = await _context.MotorcycleModels.FirstOrDefaultAsync(m => m.Id == id);
                if (model is null)
                {
                    return Response<object>.Failure("Modelo no encontrado.");
                }

                _context.MotorcycleModels.Remove(model);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Modelo eliminado con éxito.");
            }
            catch (DbUpdateException)
            {
                return Response<object>.Failure("No se puede eliminar el modelo porque tiene datos asociados.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<Response<object>> ToggleAsync(Guid id)
        {
            try
            {
                MotorcycleModel? model = await _context.MotorcycleModels.FirstOrDefaultAsync(m => m.Id == id);
                if (model is null)
                {
                    return Response<object>.Failure($"No existe modelo con id: {id}");
                }

                model.IsActive = !model.IsActive;
                _context.MotorcycleModels.Update(model);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Estado del modelo actualizado con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }
    }
}
