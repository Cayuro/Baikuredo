using Microsoft.AspNetCore.Identity;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.Helpers.Abstractions;

namespace Baikuredo.Web.Helpers.Implementations
{
    // Devuelve el Id del usuario que está usando la aplicación
    public class CurrentUserHelper : ICurrentUserHelper
    {
        private const string TEMPORARY_USER_EMAIL = "admin@baikuredo.com";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public CurrentUserHelper(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            if (principal?.Identity is not null && principal.Identity.IsAuthenticated)
            {
                string? userId = _userManager.GetUserId(principal);

                if (!string.IsNullOrEmpty(userId))
                {
                    return userId;
                }
            }

            // TEMPORAL: mientras no exista el login, se usa el usuario administrador del seeder
            User? admin = await _userManager.FindByEmailAsync(TEMPORARY_USER_EMAIL);

            if (admin is null)
            {
                throw new InvalidOperationException($"No hay sesión iniciada y no existe el usuario {TEMPORARY_USER_EMAIL}.");
            }

            return admin.Id;
        }
    }
}
