using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Roles
{
    // Sirve para crear (Id vacío) y para editar (Id con valor)
    public class RoleFormDTO
    {
        public Guid? Id { get; set; }

        [Display(Name = "Nombre del rol")]
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [MaxLength(32, ErrorMessage = "El nombre no puede superar los 32 caracteres.")]
        public string Name { get; set; } = null!;

        public List<PermissionForRoleDTO> Permissions { get; set; } = new();
    }
}
