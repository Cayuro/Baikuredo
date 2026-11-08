using Microsoft.AspNetCore.Mvc.Rendering;

namespace Baikuredo.Web.Helpers.Abstractions
{
    public interface ISelectListHelper
    {
        Task<List<SelectListItem>> GetBrandsSelectListAsync();
        Task<List<SelectListItem>> GetCategoriesSelectListAsync();
        Task<List<SelectListItem>> GetRolesSelectListAsync();
        Task<List<SelectListItem>> GetMotorcycleModelsSelectListAsync();
        Task<List<SelectListItem>> GetOwnersSelectListAsync();
        Task<List<SelectListItem>> GetMotorcyclesToPublishSelectListAsync();
        Task<List<SelectListItem>> GetAvailablePublicationsSelectListAsync();
        List<SelectListItem> GetDocumentTypesSelectList();
        List<SelectListItem> GetPublicationStatusesSelectList();
    }
}
