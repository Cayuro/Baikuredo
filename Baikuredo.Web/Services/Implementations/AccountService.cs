using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Account;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private const string InvalidCredentialsMessage = "Correo o contraseña incorrectos.";
        private const string LockedOutMessage = "Cuenta bloqueada temporalmente por varios intentos fallidos. Intenta de nuevo en unos minutos.";
        private const string UnexpectedErrorMessage = "No fue posible iniciar sesión. Intenta de nuevo.";

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;

        public AccountService(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IHttpContextAccessor httpContextAccessor,
                              DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        // ---------- AUTENTICACIÓN ----------

        public async Task<Response<SignInResult>> LoginAsync(LoginDTO dto)
        {
            try
            {
                User? user = await _userManager.FindByEmailAsync(dto.Email);

                // Mismo mensaje si el correo no existe o si la contraseña falla:
                // así no se revela qué correos están registrados.
                if (user is null)
                {
                    return Response<SignInResult>.Failure(InvalidCredentialsMessage);
                }

                // lockoutOnFailure: true bloquea la cuenta unos minutos tras varios intentos fallidos
                SignInResult result = await _signInManager.PasswordSignInAsync(
                    user, dto.Password, isPersistent: false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return Response<SignInResult>.Success(result, "Bienvenido.");
                }

                return Response<SignInResult>.Failure(result.IsLockedOut ? LockedOutMessage : InvalidCredentialsMessage);
            }
            catch (Exception)
            {
                // No se expone el detalle de la excepción en un login
                return Response<SignInResult>.Failure(UnexpectedErrorMessage);
            }
        }

        // Borra la cookie de autenticación: sin cookie la sesión queda invalidada
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public bool AccountUserIsAuthenticated()
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;

            return principal?.Identity?.IsAuthenticated == true;
        }

        // ---------- AUTORIZACIÓN ----------

        public async Task<bool> AccountUserIsAuthorizedAsync(string permissionCode)
        {
            User? user = await GetCurrentUserAsync();

            if (user?.BaikuredoRole is null)
            {
                return false;
            }

            // El rol de super administrador tiene acceso a todo
            if (user.BaikuredoRole.Name == Env.SUPER_ADMIN_ROLE_NAME)
            {
                return true;
            }

            // Para los demás roles el acceso depende de los permisos asociados al rol
            return await _context.RolePermissions.AnyAsync(rp =>
                rp.BaikuredoRoleId == user.BaikuredoRoleId &&
                rp.Permission!.Code == permissionCode);
        }

        public async Task<bool> AccountUserIsSuperAdminAsync()
        {
            User? user = await GetCurrentUserAsync();

            return user?.BaikuredoRole?.Name == Env.SUPER_ADMIN_ROLE_NAME;
        }

        // Usuario de la sesión actual, con su rol. Se busca por Id (claim de la cookie) y se consulta
        // en la base de datos en cada petición: si cambian el rol o los permisos, aplica de inmediato.
        private async Task<User?> GetCurrentUserAsync()
        {
            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            string? userId = _userManager.GetUserId(principal);

            if (userId is null)
            {
                return null;
            }

            return await _context.Users
                .AsNoTracking()
                .Include(u => u.BaikuredoRole)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
