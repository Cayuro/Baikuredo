using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
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
                var list = await _context.Categorias
                    .Select(c => new CategoriaDTO
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        Activo = c.Activo
                    })
                    .ToListAsync();

                return Response<List<CategoriaDTO>>.Success(list);
            }
            catch (Exception ex)
            {
                return Response<List<CategoriaDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateCategoriaDTO>> GetOneAsync(Guid id)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria is null)
                return Response<UpdateCategoriaDTO>.Failure($"No se encontró la categoría con id: {id}");

            var dto = new UpdateCategoriaDTO
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activo = categoria.Activo
            };

            return Response<UpdateCategoriaDTO>.Success(dto);
        }

        public async Task<Response<CategoriaDTO>> CreateAsync(CreateCategoriaDTO dto)
        {
            try
            {
                bool existe = await _context.Categorias.AnyAsync(c => c.Nombre.ToLower() == dto.Nombre.ToLower());
                if (existe)
                    return Response<CategoriaDTO>.Failure("Ya existe una categoría con ese nombre.");

                var entity = new Categoria
                {
                    Id = Guid.CreateVersion7(),
                    Nombre = dto.Nombre.Trim(),
                    Descripcion = dto.Descripcion?.Trim(),
                    Activo = true
                };

                await _context.Categorias.AddAsync(entity);
                await _context.SaveChangesAsync();

                var resultDto = new CategoriaDTO
                {
                    Id = entity.Id,
                    Nombre = entity.Nombre,
                    Descripcion = entity.Descripcion,
                    Activo = entity.Activo
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
                var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == dto.Id);
                if (categoria is null)
                    return Response<CategoriaDTO>.Failure("La categoría no existe.");

                bool existeDuplicado = await _context.Categorias
                    .AnyAsync(c => c.Nombre.ToLower() == dto.Nombre.ToLower() && c.Id != dto.Id);
                if (existeDuplicado)
                    return Response<CategoriaDTO>.Failure("Ya existe otra categoría con ese nombre.");

                categoria.Nombre = dto.Nombre.Trim();
                categoria.Descripcion = dto.Descripcion?.Trim();
                categoria.Activo = dto.Activo;

                _context.Categorias.Update(categoria);
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
                var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
                if (categoria is null)
                    return Response<object>.Failure("Categoría no encontrada.");

                _context.Categorias.Remove(categoria);
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