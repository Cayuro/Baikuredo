using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.AuditLogs;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class AuditService : IAuditService
    {
        private readonly DataContext _context;

        public AuditService(DataContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(string userId, string action, string tableName, Guid recordId, string? detail = null)
        {
            var log = new AuditLog
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                Detail = detail,
                CreatedAt = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(log);
        }

        public async Task<Response<PaginationResponse<AuditLogDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<AuditLog> query = _context.AuditLogs.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(a => a.Action.ToLower().Contains(filter)
                                          || a.TableName.ToLower().Contains(filter)
                                          || a.User!.Email!.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderByDescending(a => a.CreatedAt)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(a => new AuditLogDTO
                    {
                        Id = a.Id,
                        UserEmail = a.User!.Email!,
                        Action = a.Action,
                        TableName = a.TableName,
                        RecordId = a.RecordId,
                        Detail = a.Detail,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                var pagedList = new PagedList<AuditLogDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<AuditLogDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<AuditLogDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<AuditLogDTO>>.Failure(ex);
            }
        }
    }
}
