using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Marca;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IMarcasService
    {
        Task<Response<List<MarcaDTO>>> GetListAsync();
        Task<Response<PaginationResponse<MarcaDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<UpdateMarcaDTO>> GetOneAsync(Guid id);
        Task<Response<MarcaDTO>> CreateAsync(CreateMarcaDTO dto);
        Task<Response<MarcaDTO>> UpdateAsync(UpdateMarcaDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);
    }
}
