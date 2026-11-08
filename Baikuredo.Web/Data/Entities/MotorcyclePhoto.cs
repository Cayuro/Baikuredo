using System.ComponentModel.DataAnnotations;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Foto de una publicación
    public class MotorcyclePhoto : IId
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MotorcyclePublicationId { get; set; }
        public MotorcyclePublication? MotorcyclePublication { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = null!;

        // Portada de la publicación
        public bool IsMain { get; set; }

        // Posición en el carrusel
        public int SortOrder { get; set; }
    }
}
