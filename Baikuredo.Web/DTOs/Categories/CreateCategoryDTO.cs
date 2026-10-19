using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Categories
{
    public class CreateCategoryDTO
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede superar {1} caracteres.")]
        public required string Name { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(250, ErrorMessage = "El campo {0} no puede superar {1} caracteres.")]
        public string? Description { get; set; }
    }
}
