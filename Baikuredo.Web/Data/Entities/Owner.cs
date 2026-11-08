using System.ComponentModel.DataAnnotations;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Propietario: persona que entrega la moto para venta. No tiene cuenta en la web.
    public class Owner : IId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string DocumentType { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Document { get; set; } = null!;

        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(60)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(150)]
        public string? Address { get; set; }

        public ICollection<Motorcycle>? Motorcycles { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
