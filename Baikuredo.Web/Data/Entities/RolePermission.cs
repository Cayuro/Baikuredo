namespace Baikuredo.Web.Data.Entities
{
    public class RolePermission
    {
        public required Guid BaikuredoRoleId { get; set; }
        public required Guid PermissionId { get; set; }
        public BaikuredoRole? BaikuredoRole { get; set; }
        public Permission? Permission { get; set; }
    }
}
