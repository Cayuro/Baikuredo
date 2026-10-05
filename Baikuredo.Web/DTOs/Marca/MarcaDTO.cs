namespace Baikuredo.Web.DTOs.Marca
{
    public class MarcaDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}