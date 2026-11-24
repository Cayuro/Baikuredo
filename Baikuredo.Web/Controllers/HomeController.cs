using Microsoft.AspNetCore.Mvc;
using Baikuredo.Web.Core;
using Baikuredo.Web.DTOs.Dashboard;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IDashboardService _dashboardService;

        public HomeController(IAccountService accountService, IDashboardService dashboardService)
        {
            _accountService = accountService;
            _dashboardService = dashboardService;
        }

        // Inicio público para quien no tiene sesión; con sesión muestra el tablero con los números
        public async Task<IActionResult> Index()
        {
            DashboardDTO? dashboard = null;

            if (_accountService.AccountUserIsAuthenticated())
            {
                Response<DashboardDTO> response = await _dashboardService.GetDashboardAsync();

                if (response.IsSuccess)
                {
                    dashboard = response.Result;
                }
                else
                {
                    TempData["Error"] = response.Message;
                }
            }

            return View(dashboard);
        }

        // Se muestra cuando el usuario tiene sesión pero su rol no tiene el permiso
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
