namespace Baikuredo.Web.DTOs.Publications
{
    public class PublicationDetailDTO
    {
        public PublicationDTO Publication { get; set; } = null!;
        public int Year { get; set; }
        public int Displacement { get; set; }
        public string Color { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public List<PhotoDTO> Photos { get; set; } = new();
        public List<PriceHistoryDTO> PriceHistories { get; set; } = new();
        public List<PublicationReservationDTO> Reservations { get; set; } = new();
    }

    public class PhotoDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
    }

    public class PriceHistoryDTO
    {
        public decimal PreviousPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangedAt { get; set; }
        public string UserEmail { get; set; } = null!;
    }

    public class PublicationReservationDTO
    {
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public decimal DepositAmount { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Baikuredo.Web.Core.Enums.ReservationStatus Status { get; set; }
    }
}
