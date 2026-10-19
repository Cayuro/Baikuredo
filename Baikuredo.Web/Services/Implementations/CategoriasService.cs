using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Categories;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class CategoriasService : ICategoriasService
    {
        private readonly DataContext _context;

        public CategoriasService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<List<CategoryDTO>>> GetListAsync()
        {
            try
            {
                var list = await _context.Categories
                    .Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.name,
                        Description = c.Description,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();

                return Response<List<CategoryDTO>>.Success(list);
            }
            catch (Exception ex)
            {
                return Response<List<CategoryDTO>>.Failure(ex);
            }
        }

        public async Task<Response<PaginationResponse<CategoryDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                var query = _context.Categories.AsQueryable();

                // Filtrar por texto si el usuario envía un término de búsqueda
                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    query = query.Where(c => c.name.ToLower().Contains(request.Filter.ToLower()));
                }

                // Conteo total de registros
                int totalCount = await query.CountAsync();

                // Cálculo del total de páginas
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                // Consulta paginada
                var items = await query
                    .OrderBy(c => c.name)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.name,
                        Description = c.Description,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();

                var pagedList = new PagedList<CategoryDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                // Construcción de la respuesta paginada con el estándar de Fraynner
                var paginationResponse = new PaginationResponse<CategoryDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<CategoryDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<CategoryDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateCategoryDTO>> GetOneAsync(Guid id)
        {
            var categoria = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria is null)
                return Response<UpdateCategoryDTO>.Failure($"No se encontró la categoría con id: {id}");

            var dto = new UpdateCategoryDTO
            {
                Id = categoria.Id,
                Name = categoria.name,
                Description = categoria.Description,
                IsActive = categoria.IsActive
            };

            return Response<UpdateCategoryDTO>.Success(dto);
        }

        public async Task<Response<CategoryDTO>> CreateAsync(CreateCategoryDTO dto)
        {
            try
            {
                bool existe = await _context.Categories.AnyAsync(c => c.name.ToLower() == dto.Name.ToLower());
                if (existe)
                    return Response<CategoryDTO>.Failure("Ya existe una categoría con ese nombre.");

                var entity = new Category
                {
                    Id = Guid.CreateVersion7(),
                    name = dto.Name.Trim(),
                    Description = dto.Description?.Trim(),
                    IsActive = true
                };

                await _context.Categories.AddAsync(entity);
                await _context.SaveChangesAsync();

                var resultDto = new CategoryDTO
                {
                    Id = entity.Id,
                    Name = entity.name,
                    Description = entity.Description,
                    IsActive = entity.IsActive
                };

                return Response<CategoryDTO>.Success(resultDto, "Categoría creada exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<CategoryDTO>.Failure(ex);
            }
        }

        public async Task<Response<CategoryDTO>> UpdateAsync(UpdateCategoryDTO dto)
        {
            try
            {
                var categoria = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id);
                if (categoria is null)
                    return Response<CategoryDTO>.Failure("La categoría no existe.");

                bool existeDuplicado = await _context.Categories
                    .AnyAsync(c => c.name.ToLower() == dto.Name.ToLower() && c.Id != dto.Id);
                if (existeDuplicado)
                    return Response<CategoryDTO>.Failure("Ya existe otra categoría con ese nombre.");

                categoria.name = dto.Name.Trim();
                categoria.Description = dto.Description?.Trim();
                categoria.IsActive = dto.IsActive;

                _context.Categories.Update(categoria);
                await _context.SaveChangesAsync();

                return Response<CategoryDTO>.Success("Categoría actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<CategoryDTO>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                var categoria = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (categoria is null)
                    return Response<object>.Failure("Categoría no encontrada.");

                _context.Categories.Remove(categoria);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Categoría eliminada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }
    }
}