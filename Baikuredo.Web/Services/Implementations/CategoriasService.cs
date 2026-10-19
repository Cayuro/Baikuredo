using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Categoria;
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

        public async Task<Response<List<CategoriaDTO>>> GetListAsync()
        {
            try
            {
                var list = await _context.Categories
                    .Select(c => new CategoriaDTO
                    {
                        Id = c.Id,
                        Nombre = c.name,
                        Descripcion = c.Description,
                        Activo = c.IsActive
                    })
                    .ToListAsync();

                return Response<List<CategoriaDTO>>.Success(list);
            }
            catch (Exception ex)
            {
                return Response<List<CategoriaDTO>>.Failure(ex);
            }
        }

        public async Task<Response<PaginationResponse<CategoriaDTO>>> GetPaginatedListAsync(PaginationRequest request)
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
                    .Select(c => new CategoriaDTO
                    {
                        Id = c.Id,
                        Nombre = c.name,
                        Descripcion = c.Description,
                        Activo = c.IsActive
                    })
                    .ToListAsync();

                var pagedList = new PagedList<CategoriaDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                // Construcción de la respuesta paginada con el estándar de Fraynner
                var paginationResponse = new PaginationResponse<CategoriaDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<CategoriaDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<CategoriaDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateCategoriaDTO>> GetOneAsync(Guid id)
        {
            var categoria = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria is null)
                return Response<UpdateCategoriaDTO>.Failure($"No se encontró la categoría con id: {id}");

            var dto = new UpdateCategoriaDTO
            {
                Id = categoria.Id,
                Nombre = categoria.name,
                Descripcion = categoria.Description,
                Activo = categoria.IsActive
            };

            return Response<UpdateCategoriaDTO>.Success(dto);
        }

        public async Task<Response<CategoriaDTO>> CreateAsync(CreateCategoriaDTO dto)
        {
            try
            {
                bool existe = await _context.Categories.AnyAsync(c => c.name.ToLower() == dto.Nombre.ToLower());
                if (existe)
                    return Response<CategoriaDTO>.Failure("Ya existe una categoría con ese nombre.");

                var entity = new Category
                {
                    Id = Guid.CreateVersion7(),
                    name = dto.Nombre.Trim(),
                    Description = dto.Descripcion?.Trim(),
                    IsActive = true
                };

                await _context.Categories.AddAsync(entity);
                await _context.SaveChangesAsync();

                var resultDto = new CategoriaDTO
                {
                    Id = entity.Id,
                    Nombre = entity.name,
                    Descripcion = entity.Description,
                    Activo = entity.IsActive
                };

                return Response<CategoriaDTO>.Success(resultDto, "Categoría creada exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<CategoriaDTO>.Failure(ex);
            }
        }

        public async Task<Response<CategoriaDTO>> UpdateAsync(UpdateCategoriaDTO dto)
        {
            try
            {
                var categoria = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id);
                if (categoria is null)
                    return Response<CategoriaDTO>.Failure("La categoría no existe.");

                bool existeDuplicado = await _context.Categories
                    .AnyAsync(c => c.name.ToLower() == dto.Nombre.ToLower() && c.Id != dto.Id);
                if (existeDuplicado)
                    return Response<CategoriaDTO>.Failure("Ya existe otra categoría con ese nombre.");

                categoria.name = dto.Nombre.Trim();
                categoria.Description = dto.Descripcion?.Trim();
                categoria.IsActive = dto.Activo;

                _context.Categories.Update(categoria);
                await _context.SaveChangesAsync();

                return Response<CategoriaDTO>.Success("Categoría actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<CategoriaDTO>.Failure(ex);
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