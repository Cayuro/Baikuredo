using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Baikuredo.Web.DTOs.Motorcycles
{
    public class UpdateMotorcycleDTO
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public Guid Id { get; set; }

        [Display(Name = "Placa")]
        [Required(ErrorMessage = "La placa es obligatoria.")]
        [MaxLength(10, ErrorMessage = "La placa no puede superar los 10 caracteres.")]
        public string Plate { get; set; } = null!;

        [Display(Name = "Año")]
        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(1950, 2100, ErrorMessage = "El año debe estar entre {1} y {2}.")]
        public int? Year { get; set; }

        [Display(Name = "Cilindraje (cc)")]
        [Required(ErrorMessage = "El cilindraje es obligatorio.")]
        [Range(50, 3000, ErrorMessage = "El cilindraje debe estar entre {1} y {2} cc.")]
        public int? Displacement { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "El color es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El color no puede superar los 30 caracteres.")]
        public string Color { get; set; } = null!;

        [Display(Name = "Modelo")]
        [Required(ErrorMessage = "Debe seleccionar un modelo.")]
        public Guid? MotorcycleModelId { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        public Guid? CategoryId { get; set; }

        [Display(Name = "Propietario")]
        [Required(ErrorMessage = "Debe seleccionar un propietario.")]
        public Guid? OwnerId { get; set; }

        public List<SelectListItem>? MotorcycleModels { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? Owners { get; set; }
    }
}
