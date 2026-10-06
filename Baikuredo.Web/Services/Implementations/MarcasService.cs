using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Marca;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class MarcasService : IMarcasService
    {
        private readonly DataContext _context;

        public MarcasService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<List<MarcaDTO>>> GetListAsync()
        {
            try
            {
                var list = await _context.Marcas
                    .AsNoTracking()
                    .OrderBy(m => m.Nombre)
                    .Select(m => new MarcaDTO
                    {
                        Id = m.Id,
                        Nombre = m.Nombre,
                        Estado = m.Estado
                    })
                    .ToListAsync();

                return Response<List<MarcaDTO>>.Success(list);
            }
            catch (Exception ex)
            {
                return Response<List<MarcaDTO>>.Failure(ex);
            }
        }

        public async Task<Response<PaginationResponse<MarcaDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<Marca> query = _context.Marcas.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    query = query.Where(m => m.Nombre.ToLower().Contains(request.Filter.ToLower()));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderBy(m => m.Nombre)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(m => new MarcaDTO
                    {
                        Id = m.Id,
                        Nombre = m.Nombre,
                        Estado = m.Estado
                    })
                    .ToListAsync();

                var pagedList = new PagedList<MarcaDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<MarcaDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<MarcaDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<MarcaDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateMarcaDTO>> GetOneAsync(Guid id)
        {
            try
            {
                Marca? marca = await _context.Marcas.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (marca is null)
                {
                    return Response<UpdateMarcaDTO>.Failure($"No se encontró la marca con id: {id}");
                }

                var dto = new UpdateMarcaDTO
                {
                    Id = marca.Id,
                    Nombre = marca.Nombre,
                    Estado = marca.Estado
                };

                return Response<UpdateMarcaDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<UpdateMarcaDTO>.Failure(ex);
            }
        }

        public async Task<Response<MarcaDTO>> CreateAsync(CreateMarcaDTO dto)
        {
            try
            {
                string nombre = dto.Nombre.Trim();

                bool existe = await _context.Marcas.AnyAsync(m => m.Nombre.ToLower() == nombre.ToLower());
                if (existe)
                {
                    return Response<MarcaDTO>.Failure("Ya existe una marca con ese nombre.");
                }

                var entity = new Marca
                {
                    Id = Guid.CreateVersion7(),
                    Nombre = nombre,
                    Estado = true
                };

                await _context.Marcas.AddAsync(entity);
                await _context.SaveChangesAsync();

                var resultDto = new MarcaDTO
                {
                    Id = entity.Id,
                    Nombre = entity.Nombre,
                    Estado = entity.Estado
                };

                return Response<MarcaDTO>.Success(resultDto, "Marca creada exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<MarcaDTO>.Failure(ex);
            }
        }

        public async Task<Response<MarcaDTO>> UpdateAsync(UpdateMarcaDTO dto)
        {
            try
            {
                Marca? marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == dto.Id);
                if (marca is null)
                {
                    return Response<MarcaDTO>.Failure("La marca no existe.");
                }

                string nombre = dto.Nombre.Trim();

                bool existeDuplicado = await _context.Marcas
                    .AnyAsync(m => m.Nombre.ToLower() == nombre.ToLower() && m.Id != dto.Id);
                if (existeDuplicado)
                {
                    return Response<MarcaDTO>.Failure("Ya existe otra marca con ese nombre.");
                }

                marca.Nombre = nombre;
                marca.Estado = dto.Estado;

                _context.Marcas.Update(marca);
                await _context.SaveChangesAsync();

                return Response<MarcaDTO>.Success("Marca actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<MarcaDTO>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                Marca? marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id);
                if (marca is null)
                {
                    return Response<object>.Failure("Marca no encontrada.");
                }

                _context.Marcas.Remove(marca);
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
                Marca? marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id);
                if (marca is null)
                {
                    return Response<object>.Failure($"No existe marca con id: {id}");
                }

                marca.Estado = !marca.Estado;
                _context.Marcas.Update(marca);
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
