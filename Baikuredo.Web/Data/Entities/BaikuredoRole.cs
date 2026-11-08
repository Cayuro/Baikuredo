using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.Data.Entities
{
    public class BaikuredoRole
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(32)]
        public required string Name { get; set; }

        public IEnumerable<RolePermission>? RolePermissions { get; set; }
    }
}
