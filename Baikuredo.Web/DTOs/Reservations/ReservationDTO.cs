using Baikuredo.Web.Core.Enums;

namespace Baikuredo.Web.DTOs.Reservations
{
    public class ReservationDTO
    {
        public Guid Id { get; set; }
        public Guid MotorcyclePublicationId { get; set; }
        public string MotorcycleText { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public decimal DepositAmount { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public ReservationStatus Status { get; set; }
        public string UserEmail { get; set; } = null!;
    }
}
