using System.ComponentModel.DataAnnotations;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Interesado: persona que, desde el catálogo público, pide información sobre una publicación (F003).
    // El administrador los consulta y los marca como atendidos (F011).
    public class Interest : IId
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MotorcyclePublicationId { get; set; }
        public MotorcyclePublication? MotorcyclePublication { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [MaxLength(500)]
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Se llenan cuando un usuario del sistema atiende al interesado
        public bool IsAttended { get; set; }

        public DateTime? AttendedAt { get; set; }

        [MaxLength(450)]
        public string? AttendedByUserId { get; set; }
        public User? AttendedByUser { get; set; }
    }
}
