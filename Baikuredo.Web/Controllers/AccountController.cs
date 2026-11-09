using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baikuredo.Web.Core;
using Baikuredo.Web.DTOs.Account;
using Baikuredo.Web.Services.Abstractions;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Baikuredo.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // LOGIN - Mostrar formulario. Es público: el usuario aún no tiene sesión.
        // returnUrl llega cuando CustomAuthorize lo redirige desde una página protegida.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_accountService.AccountUserIsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        // LOGIN - Validar credenciales
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO dto, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            Response<IdentitySignInResult> response = await _accountService.LoginAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "Correo o contraseña incorrectos.");
                return View(dto);
            }

            // Solo se redirige a rutas del propio sitio (evita ataques de "open redirect")
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // LOGOUT: por POST para que un enlace externo no pueda cerrar la sesión del usuario
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();

            TempData["Success"] = "Sesión cerrada correctamente.";

            return RedirectToAction(nameof(Login));
        }
    }
}
