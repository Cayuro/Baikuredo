using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    public class MotorcycleModel : IId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = null!;

        public Guid BrandId { get; set; }

        public Brand? Brand { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
