using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Publicación: ciclo comercial de una moto que está a la venta.
    public class MotorcyclePublication : IId
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MotorcycleId { get; set; }
        public Motorcycle? Motorcycle { get; set; }

        // Usuario (Admin o Auxiliar) que publicó. En Identity el Id del usuario es string.
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = null!;
        public User? User { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public int Mileage { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public PublicationStatus Status { get; set; } = PublicationStatus.Available;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Se llena cuando pasa a Vendida o Cancelada
        public DateTime? ClosedAt { get; set; }

        public ICollection<MotorcyclePhoto>? Photos { get; set; }
        public ICollection<PriceHistory>? PriceHistories { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
