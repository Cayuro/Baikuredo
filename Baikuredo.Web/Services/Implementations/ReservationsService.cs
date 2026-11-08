using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Reservations;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    // Reglas:
    // - Solo se reserva una publicación Disponible. Al reservar, la publicación pasa a Reservada.
    // - Completar la reserva = la moto se vendió. La publicación pasa a Vendida.
    // - Expirar la reserva = el cliente no volvió. La publicación vuelve a Disponible.
    public class ReservationsService : IReservationsService
    {
        private const string TABLE_NAME = "Reservations";

        private readonly DataContext _context;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserHelper _currentUserHelper;

        public ReservationsService(DataContext context, IAuditService auditService, ICurrentUserHelper currentUserHelper)
        {
            _context = context;
            _auditService = auditService;
            _currentUserHelper = currentUserHelper;
        }

        public async Task<Response<PaginationResponse<ReservationDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<Reservation> query = _context.Reservations.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(r => r.CustomerName.ToLower().Contains(filter)
                                          || r.CustomerPhone.ToLower().Contains(filter)
                                          || r.MotorcyclePublication!.Motorcycle!.Plate.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderByDescending(r => r.ReservedAt)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(r => new ReservationDTO
                    {
                        Id = r.Id,
                        MotorcyclePublicationId = r.MotorcyclePublicationId,
                        MotorcycleText = r.MotorcyclePublication!.Motorcycle!.Plate + " - " + r.MotorcyclePublication!.Motorcycle!.MotorcycleModel!.Brand!.Name + " " + r.MotorcyclePublication!.Motorcycle!.MotorcycleModel!.Name,
                        CustomerName = r.CustomerName,
                        CustomerPhone = r.CustomerPhone,
                        DepositAmount = r.DepositAmount,
                        ReservedAt = r.ReservedAt,
                        ExpiresAt = r.ExpiresAt,
                        Status = r.Status,
                        UserEmail = r.User!.Email!
                    })
                    .ToListAsync();

                var pagedList = new PagedList<ReservationDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<ReservationDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<ReservationDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<ReservationDTO>>.Failure(ex);
            }
        }

        public async Task<Response<object>> CreateAsync(CreateReservationDTO dto)
        {
            try
            {
                MotorcyclePublication? publication = await _context.MotorcyclePublications.FirstOrDefaultAsync(p => p.Id == dto.MotorcyclePublicationId!.Value);
                if (publication is null)
                {
                    return Response<object>.Failure("La publicación no existe.");
                }

                if (publication.Status != PublicationStatus.Available)
                {
                    return Response<object>.Failure($"Solo se pueden reservar publicaciones disponibles. Esta está {publication.Status.ToSpanish()}.");
                }

                // La fecha llega sin hora: la reserva vence al final de ese día
                DateTime expiresAt = dto.ExpiresAt!.Value.Date.AddDays(1).AddSeconds(-1);
                if (expiresAt <= DateTime.Now)
                {
                    return Response<object>.Failure("La fecha límite debe ser hoy o una fecha futura.");
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();

                var reservation = new Reservation
                {
                    Id = Guid.CreateVersion7(),
                    MotorcyclePublicationId = publication.Id,
                    CustomerName = dto.CustomerName.Trim(),
                    CustomerPhone = dto.CustomerPhone.Trim(),
                    DepositAmount = dto.DepositAmount!.Value,
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt.ToUniversalTime(),
                    Status = ReservationStatus.Active,
                    UserId = userId
                };

                publication.Status = PublicationStatus.Reserved;

                await _context.Reservations.AddAsync(reservation);
                await _auditService.AddLogAsync(userId, "RESERVAR", TABLE_NAME, reservation.Id, $"Cliente {reservation.CustomerName}, anticipo {reservation.DepositAmount:N0}");
                await _context.SaveChangesAsync();

                return Response<object>.Success("Reserva creada. La publicación quedó como Reservada.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<Response<object>> CompleteAsync(Guid id)
        {
            return await CloseReservationAsync(id, ReservationStatus.Completed, PublicationStatus.Sold, "VENDER", "Reserva completada. La moto quedó como Vendida.");
        }

        public async Task<Response<object>> ExpireAsync(Guid id)
        {
            return await CloseReservationAsync(id, ReservationStatus.Expired, PublicationStatus.Available, "EXPIRAR_RESERVA", "Reserva expirada. La publicación volvió a estar Disponible.");
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                Reservation? reservation = await _context.Reservations.Include(r => r.MotorcyclePublication)
                                                                     .FirstOrDefaultAsync(r => r.Id == id);
                if (reservation is null)
                {
                    return Response<object>.Failure("Reserva no encontrada.");
                }

                // Si se borra una reserva vigente, la publicación se libera
                if (reservation.Status == ReservationStatus.Active && reservation.MotorcyclePublication is not null)
                {
                    reservation.MotorcyclePublication.Status = PublicationStatus.Available;
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();

                _context.Reservations.Remove(reservation);
                await _auditService.AddLogAsync(userId, "ELIMINAR", TABLE_NAME, reservation.Id, $"Cliente {reservation.CustomerName}");
                await _context.SaveChangesAsync();

                return Response<object>.Success("Reserva eliminada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        // Cierra una reserva vigente y cambia el estado de su publicación
        private async Task<Response<object>> CloseReservationAsync(Guid id, ReservationStatus newStatus, PublicationStatus publicationStatus, string action, string message)
        {
            try
            {
                Reservation? reservation = await _context.Reservations.Include(r => r.MotorcyclePublication)
                                                                     .FirstOrDefaultAsync(r => r.Id == id);
                if (reservation is null || reservation.MotorcyclePublication is null)
                {
                    return Response<object>.Failure("Reserva no encontrada.");
                }

                if (reservation.Status != ReservationStatus.Active)
                {
                    return Response<object>.Failure($"La reserva ya está {reservation.Status.ToSpanish()}.");
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();

                reservation.Status = newStatus;
                reservation.MotorcyclePublication.Status = publicationStatus;
                reservation.MotorcyclePublication.ClosedAt = publicationStatus == PublicationStatus.Sold ? DateTime.UtcNow : null;

                await _auditService.AddLogAsync(userId, action, TABLE_NAME, reservation.Id, $"Cliente {reservation.CustomerName}");
                await _context.SaveChangesAsync();

                return Response<object>.Success(message);
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }
    }
}
