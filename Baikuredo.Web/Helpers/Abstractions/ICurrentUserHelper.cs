namespace Baikuredo.Web.Helpers.Abstractions
{
    public interface ICurrentUserHelper
    {
        Task<string> GetCurrentUserIdAsync();
    }
}
