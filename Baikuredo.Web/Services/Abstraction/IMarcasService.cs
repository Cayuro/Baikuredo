using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Brands;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IMarcasService
    {
        Task<Response<List<BrandDTO>>> GetListAsync();
        Task<Response<PaginationResponse<BrandDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<UpdateBrandDTO>> GetOneAsync(Guid id);
        Task<Response<BrandDTO>> CreateAsync(CreateBrandDTO dto);
        Task<Response<BrandDTO>> UpdateAsync(UpdateBrandDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
        Task<Response<object>> ToggleAsync(Guid id);
    }
}
