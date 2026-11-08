using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Reserva (apartado) de una moto con un anticipo
    public class Reservation : IId
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MotorcyclePublicationId { get; set; }
        public MotorcyclePublication? MotorcyclePublication { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; }

        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        // Plazo máximo antes de liberar la moto
        public DateTime ExpiresAt { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Active;

        // Usuario (auxiliar) que atendió la reserva
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = null!;
        public User? User { get; set; }
    }
}
