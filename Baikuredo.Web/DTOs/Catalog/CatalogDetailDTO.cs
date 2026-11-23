using Baikuredo.Web.DTOs.Interests;
using Baikuredo.Web.DTOs.Publications;

namespace Baikuredo.Web.DTOs.Catalog
{
    // Detalle público de una publicación con todas sus fotos y el formulario de interés (F002, F003).
    // No incluye datos internos como propietario, placa, usuario publicador, historial de precios ni reservas.
    public class CatalogDetailDTO
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
        public DateTime CreatedAt { get; set; }
        public List<PhotoDTO> Photos { get; set; } = new();
        public CreateInterestDTO Interest { get; set; } = new();
    }
}
