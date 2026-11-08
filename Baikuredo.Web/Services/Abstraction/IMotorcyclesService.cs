using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Motorcycles;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IMotorcyclesService
    {
        Task<Response<PaginationResponse<MotorcycleDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<UpdateMotorcycleDTO>> GetOneAsync(Guid id);
        Task<Response<MotorcycleDTO>> GetDetailAsync(Guid id);
        Task<Response<MotorcycleDTO>> CreateAsync(CreateMotorcycleDTO dto);
        Task<Response<MotorcycleDTO>> UpdateAsync(UpdateMotorcycleDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
    }
}
