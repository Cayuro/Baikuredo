using System.ComponentModel.DataAnnotations;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Bitácora: quién hizo qué, sobre qué registro y cuándo
    public class AuditLog : IId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = null!;
        public User? User { get; set; }

        // Ej: CREAR, MODIFICAR_PRECIO, VENDER, ELIMINAR
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string TableName { get; set; } = null!;

        public Guid RecordId { get; set; }

        [MaxLength(500)]
        public string? Detail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
