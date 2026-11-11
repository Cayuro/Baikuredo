using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Account
{
    public class ChangePasswordDTO
    {
        [Display(Name = "Contraseña actual")]
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Display(Name = "Nueva contraseña")]
        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(4, ErrorMessage = "La nueva contraseña debe tener al menos 4 caracteres.")]
        [MaxLength(100, ErrorMessage = "La nueva contraseña no puede superar los 100 caracteres.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Display(Name = "Confirmar nueva contraseña")]
        [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
