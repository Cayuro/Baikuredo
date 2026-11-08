using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Baikuredo.Web.DTOs.Reservations
{
    public class CreateReservationDTO
    {
        [Display(Name = "Publicación")]
        [Required(ErrorMessage = "Debe seleccionar una publicación.")]
        public Guid? MotorcyclePublicationId { get; set; }

        [Display(Name = "Nombre del cliente")]
        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string CustomerName { get; set; } = null!;

        [Display(Name = "Teléfono del cliente")]
        [Required(ErrorMessage = "El teléfono del cliente es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string CustomerPhone { get; set; } = null!;

        [Display(Name = "Anticipo")]
        [Required(ErrorMessage = "El anticipo es obligatorio.")]
        [Range(0, 9999999999, ErrorMessage = "El anticipo no puede ser negativo.")]
        public decimal? DepositAmount { get; set; }

        [Display(Name = "Fecha límite")]
        [Required(ErrorMessage = "La fecha límite es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime? ExpiresAt { get; set; }

        public List<SelectListItem>? Publications { get; set; }
    }
}
