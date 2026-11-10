namespace Baikuredo.Web.DTOs.Roles
{
    public class RoleListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int UsersCount { get; set; }
        public int PermissionsCount { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
