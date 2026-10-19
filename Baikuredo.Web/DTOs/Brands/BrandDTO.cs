namespace Baikuredo.Web.DTOs.Brands
{
    public class BrandDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool isActive { get; set; }
    }
}