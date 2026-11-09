using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Baikuredo.Web.DTOs.Users
{
    public class CreateUserDTO
    {
        [Display(Name = "Documento")]
        [Required(ErrorMessage = "El documento es obligatorio.")]
        [MaxLength(32, ErrorMessage = "El documento no puede superar los 32 caracteres.")]
        public string Document { get; set; } = null!;

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [MaxLength(64, ErrorMessage = "Los nombres no pueden superar los 64 caracteres.")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [MaxLength(64, ErrorMessage = "Los apellidos no pueden superar los 64 caracteres.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [MaxLength(256, ErrorMessage = "El correo no puede superar los 256 caracteres.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Teléfono")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Phone { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public Guid? BaikuredoRoleId { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres.")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Confirmar contraseña")]
        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = null!;

        public List<SelectListItem> Roles { get; set; } = new();
    }
}
