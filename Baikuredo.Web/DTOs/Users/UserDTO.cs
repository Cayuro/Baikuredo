namespace Baikuredo.Web.DTOs.Users
{
    public class UserDTO
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string? Phone { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
