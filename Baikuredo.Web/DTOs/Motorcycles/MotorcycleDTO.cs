namespace Baikuredo.Web.DTOs.Motorcycles
{
    public class MotorcycleDTO
    {
        public Guid Id { get; set; }
        public string Plate { get; set; } = null!;
        public int Year { get; set; }
        public int Displacement { get; set; }
        public string Color { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string ModelName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
    }
}
