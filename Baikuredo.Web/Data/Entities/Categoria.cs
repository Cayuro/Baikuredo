using System.ComponentModel.DataAnnotations;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    public class Categoria : IId
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
        public required string Nombre { get; set; }

        [MaxLength(250, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
