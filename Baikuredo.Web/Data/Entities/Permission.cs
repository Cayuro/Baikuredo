using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.Data.Entities
{
    public class Permission
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(32)]
        public required string Code { get; set; }

        [Required]
        [MaxLength(256)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(32)]
        public required string Module { get; set; }

        public IEnumerable<RolePermission>? RolePermissions { get; set; }
    }
}
