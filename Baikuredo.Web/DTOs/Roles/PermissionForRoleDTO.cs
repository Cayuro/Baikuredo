namespace Baikuredo.Web.DTOs.Roles
{
    // Una casilla del formulario de roles
    public class PermissionForRoleDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Module { get; set; } = null!;
        public bool Selected { get; set; }
    }
}
