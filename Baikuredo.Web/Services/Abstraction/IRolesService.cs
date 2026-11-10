using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Roles;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IRolesService
    {
        Task<Response<PaginationResponse<RoleListDTO>>> GetPaginatedListAsync(PaginationRequest request);

        // Formulario vacío para crear: todos los permisos sin marcar
        Task<Response<RoleFormDTO>> GetNewFormAsync();

        // Formulario para editar: los permisos del rol vienen marcados
        Task<Response<RoleFormDTO>> GetOneAsync(Guid id);

        Task<Response<object>> CreateAsync(RoleFormDTO dto);
        Task<Response<object>> UpdateAsync(RoleFormDTO dto);
        Task<Response<object>> DeleteAsync(Guid id);

        // Vuelve a cargar los textos de los permisos si el formulario se devuelve con errores
        Task<List<PermissionForRoleDTO>> RebuildPermissionsAsync(List<PermissionForRoleDTO> posted);
    }
}
