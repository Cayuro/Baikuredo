using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Marca
{
    public class CreateMarcaDTO
    {
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = null!;
    }
}