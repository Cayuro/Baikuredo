using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.MotorcycleModels;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IMotorcycleModelsService
    {
        Task<Response<PaginationResponse<MotorcycleModelDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<UpdateMotorcycleModelDTO>> GetOneAsync(Guid id);
        Task<Response<MotorcycleModelDTO>> CreateAsync(CreateMotorcycleModelDTO dto);
        Task<Response<MotorcycleModelDTO>> UpdateAsync(UpdateMotorcycleModelDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
        Task<Response<object>> ToggleAsync(Guid id);
    }
}
