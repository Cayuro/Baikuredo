using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.MotorcycleModels
{
    public class UpdateMotorcycleModelDTO
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre del modelo es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Name { get; set; } = null!;

        [Display(Name = "Marca")]
        [Required(ErrorMessage = "Debe seleccionar una marca.")]
        public Guid? BrandId { get; set; }

        public string? BrandName { get; set; }

        public bool IsActive { get; set; }
    }
}