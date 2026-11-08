using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.AuditLogs;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IAuditService
    {
        // Agrega un registro a la bitácora. No guarda: se guarda junto con el SaveChanges del servicio que lo llama.
        Task AddLogAsync(string userId, string action, string tableName, Guid recordId, string? detail = null);
        Task<Response<PaginationResponse<AuditLogDTO>>> GetPaginatedListAsync(PaginationRequest request);
    }
}
