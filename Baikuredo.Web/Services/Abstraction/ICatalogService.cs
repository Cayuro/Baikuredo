using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Catalog;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface ICatalogService
    {
        // Lista paginada solo de publicaciones en estado "Disponible" con su foto de portada (F001)
        Task<Response<PaginationResponse<CatalogItemDTO>>> GetPaginatedListAsync(PaginationRequest request);

        // Detalle público de una publicación "Disponible" con todas sus fotos, sin datos internos (F002)
        Task<Response<CatalogDetailDTO>> GetDetailAsync(Guid id);
    }
}
