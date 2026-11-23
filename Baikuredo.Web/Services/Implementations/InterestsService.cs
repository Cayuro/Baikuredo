using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Interests;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    // Reglas:
    // - Un visitante solo puede mostrar interés en una publicación Disponible.
    // - Atender o eliminar a un interesado queda en la auditoría.
    public class InterestsService : IInterestsService
    {
        private const string TABLE_NAME = "Interests";

        private readonly DataContext _context;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserHelper _currentUserHelper;

        public InterestsService(DataContext context, IAuditService auditService, ICurrentUserHelper currentUserHelper)
        {
            _context = context;
            _auditService = auditService;
            _currentUserHelper = currentUserHelper;
        }

        public async Task<Response<PaginationResponse<InterestDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<Interest> query = _context.Interests.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(i => i.FullName.ToLower().Contains(filter)
                                          || i.Email.ToLower().Contains(filter)
                                          || i.Phone.ToLower().Contains(filter)
                                          || i.MotorcyclePublication!.Motorcycle!.Plate.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                // Primero los pendientes y luego los más recientes
                var items = await query
                    .OrderBy(i => i.IsAttended)
                    .ThenByDescending(i => i.CreatedAt)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(i => new InterestDTO
                    {
                        Id = i.Id,
                        MotorcyclePublicationId = i.MotorcyclePublicationId,
                        MotorcycleText = i.MotorcyclePublication!.Motorcycle!.Plate + " - " + i.MotorcyclePublication!.Motorcycle!.MotorcycleModel!.Brand!.Name + " " + i.MotorcyclePublication!.Motorcycle!.MotorcycleModel!.Name,
                        FullName = i.FullName,
                        Email = i.Email,
                        Phone = i.Phone,
                        Message = i.Message,
                        CreatedAt = i.CreatedAt,
                        IsAttended = i.IsAttended,
                        AttendedAt = i.AttendedAt,
                        AttendedByEmail = i.AttendedByUser != null ? i.AttendedByUser.Email : null
                    })
                    .ToListAsync();

                var pagedList = new PagedList<InterestDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<InterestDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<InterestDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<InterestDTO>>.Failure(ex);
            }
        }

        public async Task<Response<object>> CreateAsync(CreateInterestDTO dto)
        {
            try
            {
                MotorcyclePublication? publication = await _context.MotorcyclePublications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == dto.MotorcyclePublicationId);

                if (publication is null)
                {
                    return Response<object>.Failure("La publicación no existe.");
                }

                if (publication.Status != PublicationStatus.Available)
                {
                    return Response<object>.Failure("Esta moto ya no está disponible.");
                }

                var entity = new Interest
                {
                    Id = Guid.CreateVersion7(),
                    MotorcyclePublicationId = dto.MotorcyclePublicationId,
                    FullName = dto.FullName.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    Phone = dto.Phone.Trim(),
                    Message = string.IsNullOrWhiteSpace(dto.Message) ? null : dto.Message.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Interests.AddAsync(entity);
                await _context.SaveChangesAsync();

                return Response<object>.Success("¡Gracias! Recibimos tus datos y pronto te contactaremos.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<Response<object>> ToggleAttendedAsync(Guid id)
        {
            try
            {
                Interest? interest = await _context.Interests.FirstOrDefaultAsync(i => i.Id == id);

                if (interest is null)
                {
                    return Response<object>.Failure($"No existe interesado con id: {id}");
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();

                interest.IsAttended = !interest.IsAttended;

                if (interest.IsAttended)
                {
                    interest.AttendedAt = DateTime.UtcNow;
                    interest.AttendedByUserId = userId;
                }
                else
                {
                    interest.AttendedAt = null;
                    interest.AttendedByUserId = null;
                }

                string action = interest.IsAttended ? "ATENDER" : "REABRIR";
                await _auditService.AddLogAsync(userId, action, TABLE_NAME, interest.Id, interest.FullName);

                await _context.SaveChangesAsync();

                string message = interest.IsAttended ? "Interesado marcado como atendido." : "Interesado marcado como pendiente.";
                return Response<object>.Success(message);
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                Interest? interest = await _context.Interests.FirstOrDefaultAsync(i => i.Id == id);

                if (interest is null)
                {
                    return Response<object>.Failure($"No existe interesado con id: {id}");
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();

                _context.Interests.Remove(interest);
                await _auditService.AddLogAsync(userId, "ELIMINAR", TABLE_NAME, interest.Id, interest.FullName);

                await _context.SaveChangesAsync();

                return Response<object>.Success("Interesado eliminado con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }
    }
}
