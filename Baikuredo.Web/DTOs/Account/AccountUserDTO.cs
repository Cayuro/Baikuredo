using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Baikuredo.Web.DTOs.Account
{
    // Datos del usuario que tiene la sesión abierta. No lleva Id: el usuario siempre se toma de la sesión,
    // así nadie puede editar la cuenta de otra persona cambiando un valor del formulario.
    public class AccountUserDTO
    {
        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(64, ErrorMessage = "Los nombres no pueden superar los 64 caracteres.")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(64, ErrorMessage = "Los apellidos no pueden superar los 64 caracteres.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Documento")]
        [Required(ErrorMessage = "El documento es obligatorio.")]
        [MaxLength(32, ErrorMessage = "El documento no puede superar los 32 caracteres.")]
        public string Document { get; set; } = null!;

        [Display(Name = "Teléfono")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Phone { get; set; }

        // Solo lectura: se muestran pero no se editan desde "Mi cuenta"
        // (el correo es el usuario de acceso y el rol lo asigna un administrador).
        [BindNever]
        [Display(Name = "Correo")]
        public string? Email { get; set; }

        [BindNever]
        [Display(Name = "Rol")]
        public string? RoleName { get; set; }
    }
}
