namespace Baikuredo.Web.DTOs.Owners
{
    public class OwnerDTO
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
