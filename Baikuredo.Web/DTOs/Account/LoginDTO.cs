using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo válido.")]
        [Display(Name = "Correo")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public required string Password { get; set; }
    }
}
