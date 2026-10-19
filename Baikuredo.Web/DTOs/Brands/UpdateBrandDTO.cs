using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Brands
{
    public class UpdateBrandDTO
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}