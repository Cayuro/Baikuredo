using System.ComponentModel.DataAnnotations;

namespace Baikuredo.Web.DTOs.Categoria
{
    public class UpdateCategoriaDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede superar {1} caracteres.")]
        public required string Nombre { get; set; }

        [MaxLength(250, ErrorMessage = "El campo {0} no puede superar {1} caracteres.")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; }
    }
}
