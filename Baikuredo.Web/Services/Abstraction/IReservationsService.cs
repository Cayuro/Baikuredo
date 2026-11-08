using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Reservations;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IReservationsService
    {
        Task<Response<PaginationResponse<ReservationDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<object>> CreateAsync(CreateReservationDTO dto);
        Task<Response<object>> CompleteAsync(Guid id);
        Task<Response<object>> ExpireAsync(Guid id);
        Task<Response<object>> DeleteAsync(Guid id);
    }
}
