namespace Baikuredo.Web.DTOs.Interests
{
    // Lo que ve el administrador en la lista de interesados (F011)
    public class InterestDTO
    {
        public Guid Id { get; set; }
        public Guid MotorcyclePublicationId { get; set; }
        public string MotorcycleText { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAttended { get; set; }
        public DateTime? AttendedAt { get; set; }
        public string? AttendedByEmail { get; set; }
    }
}
