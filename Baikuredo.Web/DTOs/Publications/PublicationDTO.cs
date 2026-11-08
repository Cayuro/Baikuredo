using Baikuredo.Web.Core.Enums;

namespace Baikuredo.Web.DTOs.Publications
{
    public class PublicationDTO
    {
        public Guid Id { get; set; }
        public Guid MotorcycleId { get; set; }
        public string Plate { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string ModelName { get; set; } = null!;
        public decimal SalePrice { get; set; }
        public int Mileage { get; set; }
        public string? Description { get; set; }
        public PublicationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string UserEmail { get; set; } = null!;
        public string? MainPhotoUrl { get; set; }
    }
}
