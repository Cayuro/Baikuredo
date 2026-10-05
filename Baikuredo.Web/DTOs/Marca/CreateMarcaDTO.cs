using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Marca
{
    public class CreateMarcaDTO
    {
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; }
    }
}