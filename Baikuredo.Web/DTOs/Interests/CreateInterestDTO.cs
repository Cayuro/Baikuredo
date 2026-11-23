using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Interests
{
    // Formulario "Me interesa" del catálogo público (F003). Lo llena un visitante sin iniciar sesión.
    public class CreateInterestDTO
    {
        [Required(ErrorMessage = "La publicación es obligatoria.")]
        public Guid MotorcyclePublicationId { get; set; }

        [Display(Name = "Nombre completo")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [MaxLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Mensaje")]
        [MaxLength(500, ErrorMessage = "El mensaje no puede superar los 500 caracteres.")]
        public string? Message { get; set; }
    }
}
