using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    public class Marca : IId
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Nombre { get; set; } = null!;

        [Required]
        public bool Estado { get; set; }
    }
}
