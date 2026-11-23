namespace Baikuredo.Web.DTOs.Catalog
{
    // Tarjeta de una publicación disponible en el catálogo público (F001)
    public class CatalogItemDTO
    {
        public Guid Id { get; set; }
        public string BrandName { get; set; } = null!;
        public string ModelName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int Year { get; set; }
        public int Displacement { get; set; }
        public string Color { get; set; } = null!;
        public decimal SalePrice { get; set; }
        public int Mileage { get; set; }
        public string? Description { get; set; }
        public string? MainPhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
