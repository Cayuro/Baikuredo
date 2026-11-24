using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Data;
using Baikuredo.Web.DTOs.Dashboard;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly DataContext _context;
        private readonly IAccountService _accountService;

        public DashboardService(DataContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public async Task<Response<DashboardDTO>> GetDashboardAsync()
        {
            try
            {
                var dto = new DashboardDTO();

                // Cada número usa el mismo permiso "modulo.show" que el menú lateral:
                // si el rol no puede ver el módulo, el contador se queda en null y no se consulta.
                if (await _accountService.AccountUserIsAuthorizedAsync("publications.show"))
                {
                    dto.AvailablePublications = await _context.MotorcyclePublications
                        .CountAsync(p => p.Status == PublicationStatus.Available);
                }

                if (await _accountService.AccountUserIsAuthorizedAsync("motorcycles.show"))
                {
                    dto.RegisteredMotorcycles = await _context.Motorcycles.CountAsync();
                }

                if (await _accountService.AccountUserIsAuthorizedAsync("interests.show"))
                {
                    dto.PendingInterests = await _context.Interests
                        .CountAsync(i => !i.IsAttended);
                }

                if (await _accountService.AccountUserIsAuthorizedAsync("reservations.show"))
                {
                    dto.ActiveReservations = await _context.Reservations
                        .CountAsync(r => r.Status == ReservationStatus.Active);
                }

                return Response<DashboardDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<DashboardDTO>.Failure(ex);
            }
        }
    }
}
