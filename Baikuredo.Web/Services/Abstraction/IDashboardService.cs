using Baikuredo.Web.Core;
using Baikuredo.Web.DTOs.Dashboard;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IDashboardService
    {
        // Cuenta lo que el usuario de la sesión actual tiene permiso de ver
        Task<Response<DashboardDTO>> GetDashboardAsync();
    }
}
