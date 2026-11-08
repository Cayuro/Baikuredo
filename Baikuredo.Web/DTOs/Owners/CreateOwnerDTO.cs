using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Baikuredo.Web.DTOs.Owners
{
    public class CreateOwnerDTO
    {
        [Display(Name = "Tipo de documento")]
        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        [MaxLength(10, ErrorMessage = "El tipo de documento no puede superar los 10 caracteres.")]
        public string DocumentType { get; set; } = null!;

        [Display(Name = "Documento")]
        [Required(ErrorMessage = "El documento es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El documento no puede superar los 20 caracteres.")]
        public string Document { get; set; } = null!;

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(60, ErrorMessage = "Los nombres no pueden superar los 60 caracteres.")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(60, ErrorMessage = "Los apellidos no pueden superar los 60 caracteres.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Correo")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [MaxLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        public string? Email { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
        public string? Address { get; set; }

        public List<SelectListItem>? DocumentTypes { get; set; }
    }
}
