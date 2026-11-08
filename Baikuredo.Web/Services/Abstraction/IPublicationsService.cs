using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Publications;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IPublicationsService
    {
        Task<Response<PaginationResponse<PublicationDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<UpdatePublicationDTO>> GetOneAsync(Guid id);
        Task<Response<PublicationDetailDTO>> GetDetailAsync(Guid id);
        Task<Response<Guid>> CreateAsync(CreatePublicationDTO dto);
        Task<Response<object>> UpdateAsync(UpdatePublicationDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);

        // Fotos: devuelven el Id de la publicación para volver a su detalle
        Task<Response<Guid>> AddPhotoAsync(Guid publicationId, IFormFile file);
        Task<Response<Guid>> DeletePhotoAsync(Guid photoId);
        Task<Response<Guid>> SetMainPhotoAsync(Guid photoId);
    }
}
