using System.ComponentModel.DataAnnotations;

namespace DTOs.Categoria
{
    public class UpdateCategoriaDTO
    {
        [Required(ErrorMessage = "El identificador es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El identificador debe ser un número mayor a cero.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado activo es obligatorio.")]
        public bool Activo { get; set; }
    }
}
