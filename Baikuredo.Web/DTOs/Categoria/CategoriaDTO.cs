namespace Baikuredo.Web.DTOs.Categoria
{
    public class CategoriaDTO
    {
        public Guid Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
