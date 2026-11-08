namespace Baikuredo.Web.Services.Abstractions
{
    public interface IAccountService
    {
        bool AccountUserIsAuthenticated();
        Task<bool> AccountUserIsAuthorizedAsync(string permissionCode);
        Task<bool> AccountUserIsSuperAdminAsync();
    }
}
