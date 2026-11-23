using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Interests;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IInterestsService
    {
        // Lista para el administrador (F011). El filtro busca por nombre, correo, teléfono o placa.
        Task<Response<PaginationResponse<InterestDTO>>> GetPaginatedListAsync(PaginationRequest request);

        // Lo usa el catálogo público: un visitante registra su interés (F003)
        Task<Response<object>> CreateAsync(CreateInterestDTO dto);

        // Marca o desmarca al interesado como atendido
        Task<Response<object>> ToggleAttendedAsync(Guid id);

        Task<Response<object>> DeleteAsync(Guid id);
    }
}
