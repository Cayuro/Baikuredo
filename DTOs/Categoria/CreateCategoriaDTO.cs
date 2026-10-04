using System.ComponentModel.DataAnnotations;

namespace DTOs.Categoria
{
    public class CreateCategoriaDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
