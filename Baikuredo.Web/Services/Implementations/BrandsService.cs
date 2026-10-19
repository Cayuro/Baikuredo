using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Brands;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class BrandsService : IBrandsService
    {
        private readonly DataContext _context;

        public BrandsService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<List<BrandDTO>>> GetListAsync()
        {
            try
            {
                var list = await _context.Brands
                    .AsNoTracking()
                    .OrderBy(m => m.name)
                    .Select(m => new BrandDTO
                    {
                        Id = m.Id,
                        Name = m.name,
                        isActive = m.isActive
                    })
                    .ToListAsync();

                return Response<List<BrandDTO>>.Success(list);
            }
            catch (Exception ex)
            {
                return Response<List<BrandDTO>>.Failure(ex);
            }
        }

        public async Task<Response<PaginationResponse<BrandDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<Brand> query = _context.Brands.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    query = query.Where(m => m.name.ToLower().Contains(request.Filter.ToLower()));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderBy(m => m.name)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(m => new BrandDTO
                    {
                        Id = m.Id,
                        Name = m.name,
                        isActive = m.isActive
                    })
                    .ToListAsync();

                var pagedList = new PagedList<BrandDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<BrandDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<BrandDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<BrandDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateBrandDTO>> GetOneAsync(Guid id)
        {
            try
            {
                Brand? brand = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (brand is null)
                {
                    return Response<UpdateBrandDTO>.Failure($"No se encontró la marca con id: {id}");
                }

                var dto = new UpdateBrandDTO
                {
                    Id = brand.Id,
                    Name = brand.name,
                    IsActive = brand.isActive
                };

                return Response<UpdateBrandDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<UpdateBrandDTO>.Failure(ex);
            }
        }

        public async Task<Response<BrandDTO>> CreateAsync(CreateBrandDTO dto)
        {
            try
            {
                string name = dto.Name.Trim();

                bool exists = await _context.Brands.AnyAsync(m => m.name.ToLower() == name.ToLower());
                if (exists)
                {
                    return Response<BrandDTO>.Failure("Ya existe una marca con ese nombre.");
                }

                var entity = new Brand
                {
                    Id = Guid.CreateVersion7(),
                    name = name,
                    isActive = true
                };

                await _context.Brands.AddAsync(entity);
                await _context.SaveChangesAsync();

                var resultDto = new BrandDTO
                {
                    Id = entity.Id,
                    Name = entity.name,
                    isActive = entity.isActive
                };

                return Response<BrandDTO>.Success(resultDto, "Marca creada exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<BrandDTO>.Failure(ex);
            }
        }

        public async Task<Response<BrandDTO>> UpdateAsync(UpdateBrandDTO dto)
        {
            try
            {
                Brand? brand = await _context.Brands.FirstOrDefaultAsync(m => m.Id == dto.Id);
                if (brand is null)
                {
                    return Response<BrandDTO>.Failure("La marca no existe.");
                }

                string name = dto.Name.Trim();

                bool duplicateExists = await _context.Brands
                    .AnyAsync(m => m.name.ToLower() == name.ToLower() && m.Id != dto.Id);
                if (duplicateExists)
                {
                    return Response<BrandDTO>.Failure("Ya existe otra marca con ese nombre.");
                }

                brand.name = name;
                brand.isActive = dto.IsActive;

                _context.Brands.Update(brand);
                await _context.SaveChangesAsync();

                return Response<BrandDTO>.Success("Marca actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<BrandDTO>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                Brand? brand = await _context.Brands.FirstOrDefaultAsync(m => m.Id == id);
                if (brand is null)
                {
                    return Response<object>.Failure("Marca no encontrada.");
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Marca eliminada con éxito.");
            }
            catch (DbUpdateException)
            {
                return Response<object>.Failure("No se puede eliminar la marca porque tiene datos asociados.");
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
                Brand? brand = await _context.Brands.FirstOrDefaultAsync(m => m.Id == id);
                if (brand is null)
                {
                    return Response<object>.Failure($"No existe marca con id: {id}");
                }

                brand.isActive = !brand.isActive;
                _context.Brands.Update(brand);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Estado de la marca actualizado con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }
    }
}
