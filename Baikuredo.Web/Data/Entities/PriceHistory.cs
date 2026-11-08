using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Historial de cambios de precio de una publicación
    public class PriceHistory : IId
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MotorcyclePublicationId { get; set; }
        public MotorcyclePublication? MotorcyclePublication { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PreviousPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NewPrice { get; set; }

        // Usuario que hizo el cambio
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = null!;
        public User? User { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
