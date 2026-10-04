using Baikuredo.Web.Core;
using Baikuredo.Web.DTOs.Categoria;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface ICategoriasService
    {
        Task<Response<List<CategoriaDTO>>> GetListAsync();
        Task<Response<UpdateCategoriaDTO>> GetOneAsync(Guid id);
        Task<Response<CategoriaDTO>> CreateAsync(CreateCategoriaDTO dto);
        Task<Response<CategoriaDTO>> UpdateAsync(UpdateCategoriaDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
    }
}