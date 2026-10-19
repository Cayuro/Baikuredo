using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Categories;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface ICategoriasService
    {
        // Método actual sin paginar
        Task<Response<List<CategoryDTO>>> GetListAsync();

        // Nuevo método paginado
        Task<Response<PaginationResponse<CategoryDTO>>> GetPaginatedListAsync(PaginationRequest request);

        Task<Response<UpdateCategoryDTO>> GetOneAsync(Guid id);
        Task<Response<CategoryDTO>> CreateAsync(CreateCategoryDTO dto);
        Task<Response<CategoryDTO>> UpdateAsync(UpdateCategoryDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
    }
}