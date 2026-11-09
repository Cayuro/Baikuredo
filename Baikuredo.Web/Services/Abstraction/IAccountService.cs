using Microsoft.AspNetCore.Identity;
using Baikuredo.Web.Core;
using Baikuredo.Web.DTOs.Account;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IAccountService
    {
        // Autenticación
        Task<Response<SignInResult>> LoginAsync(LoginDTO dto);
        Task LogoutAsync();
        bool AccountUserIsAuthenticated();

        // Autorización (los usa CustomAuthorizeAttribute)
        Task<bool> AccountUserIsAuthorizedAsync(string permissionCode);
        Task<bool> AccountUserIsSuperAdminAsync();
    }
}
