namespace Baikuredo.Web.DTOs.MotorcycleModels
{
    public class MotorcycleModelDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
