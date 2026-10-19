using System.ComponentModel.DataAnnotations;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    public class Category : IId
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
        public required string name { get; set; }

        [MaxLength(250, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
