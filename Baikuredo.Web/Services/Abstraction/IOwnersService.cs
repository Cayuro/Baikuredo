using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Owners;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IOwnersService
    {
        Task<Response<PaginationResponse<OwnerDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<UpdateOwnerDTO>> GetOneAsync(Guid id);
        Task<Response<OwnerDTO>> CreateAsync(CreateOwnerDTO dto);
        Task<Response<OwnerDTO>> UpdateAsync(UpdateOwnerDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
    }
}
