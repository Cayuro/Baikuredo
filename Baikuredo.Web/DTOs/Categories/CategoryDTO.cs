namespace Baikuredo.Web.DTOs.Categories
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
