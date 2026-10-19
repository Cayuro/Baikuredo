using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Brands
{
    public class CreateBrandDTO
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Name { get; set; } = null!;
    }
}