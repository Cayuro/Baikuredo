using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Baikuredo.Web.DTOs.Users
{
    public class UpdateUserDTO : IValidatableObject
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string Id { get; set; } = null!;

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

        [Display(Name = "Nueva contraseña")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        public string? Password { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool hasPassword = !string.IsNullOrWhiteSpace(Password);
            bool hasConfirm = !string.IsNullOrWhiteSpace(ConfirmPassword);

            if (!hasPassword && !hasConfirm)
            {
                yield break;
            }

            if (!hasPassword || Password!.Trim().Length < 4)
            {
                yield return new ValidationResult(
                    "La contraseña debe tener al menos 4 caracteres.",
                    new[] { nameof(Password) });
            }

            if (Password != ConfirmPassword)
            {
                yield return new ValidationResult(
                    "Las contraseñas no coinciden.",
                    new[] { nameof(ConfirmPassword) });
            }
        }
    }
}
