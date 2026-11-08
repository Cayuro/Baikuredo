using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Baikuredo.Web.DTOs.Publications
{
    public class CreatePublicationDTO
    {
        [Display(Name = "Moto")]
        [Required(ErrorMessage = "Debe seleccionar una moto.")]
        public Guid? MotorcycleId { get; set; }

        [Display(Name = "Precio de venta")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(1, 9999999999, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal? SalePrice { get; set; }

        [Display(Name = "Kilometraje")]
        [Required(ErrorMessage = "El kilometraje es obligatorio.")]
        [Range(0, 2000000, ErrorMessage = "El kilometraje debe estar entre {1} y {2}.")]
        public int? Mileage { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string? Description { get; set; }

        public List<SelectListItem>? Motorcycles { get; set; }
    }
}
