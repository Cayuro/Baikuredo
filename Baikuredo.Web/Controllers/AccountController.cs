using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core;
using Baikuredo.Web.DTOs.Account;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // MI CUENTA - Ver mis datos.
        // Solo exige tener sesión ([Authorize]): son los datos propios del usuario, no un permiso del rol.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            Response<AccountUserDTO> response = await _accountService.GetAccountUserAsync();

            if (!response.IsSuccess || response.Result is null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction("Index", "Home");
            }

            return View(response.Result);
        }

        // MI CUENTA - Guardar mis datos
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(AccountUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadReadOnlyFieldsAsync(dto);
                return View(dto);
            }

            Response<AccountUserDTO> response = await _accountService.UpdateAccountUserAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible actualizar tus datos.");
                await LoadReadOnlyFieldsAsync(dto);
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Profile));
        }

        // MI CUENTA - Cambiar contraseña
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordDTO());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            Response<object> response = await _accountService.ChangePasswordAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible cambiar la contraseña.");
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Profile));
        }

        // El correo y el rol no viajan en el formulario: se vuelven a cargar al mostrar de nuevo la vista
        private async Task LoadReadOnlyFieldsAsync(AccountUserDTO dto)
        {
            Response<AccountUserDTO> current = await _accountService.GetAccountUserAsync();

            if (current.IsSuccess && current.Result is not null)
            {
                dto.Email = current.Result.Email;
                dto.RoleName = current.Result.RoleName;
            }
        }
    }
}