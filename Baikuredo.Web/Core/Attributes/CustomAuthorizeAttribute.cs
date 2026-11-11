using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Core.Attributes
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(string permissionCode) : base(typeof(CustomAuthorizeFilter))
        {
            Arguments = [permissionCode];
        }
    }

    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permissionCode;
        private readonly IAccountService _accountService;

        public CustomAuthorizeFilter(string permissionCode, IAccountService accountService)
        {
            _permissionCode = permissionCode;
            _accountService = accountService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            bool userIsAuthenticated = _accountService.AccountUserIsAuthenticated();

            if (!userIsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "Controller", "Account" },
                    { "Action", "Login" },
                    { "ReturnUrl", context.HttpContext.Request.Path }
                });

                return;
            }

            bool isAuthorized = await _accountService.AccountUserIsAuthorizedAsync(_permissionCode);

            if (!isAuthorized)
            {
                // Tiene sesión pero su rol no tiene el permiso: se le muestra la página de acceso denegado
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}
