using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Categoria;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface ICategoriasService
    {
        // Método actual sin paginar
        Task<Response<List<CategoriaDTO>>> GetListAsync();

        // Nuevo método paginado
        Task<Response<PaginationResponse<CategoriaDTO>>> GetPaginatedListAsync(PaginationRequest request);

        Task<Response<UpdateCategoriaDTO>> GetOneAsync(Guid id);
        Task<Response<CategoriaDTO>> CreateAsync(CreateCategoriaDTO dto);
        Task<Response<CategoriaDTO>> UpdateAsync(UpdateCategoriaDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
    }
}