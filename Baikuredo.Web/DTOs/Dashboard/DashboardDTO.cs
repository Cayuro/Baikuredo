namespace Baikuredo.Web.DTOs.Dashboard
{
    // Números del tablero de inicio.
    // Cada contador es null cuando el rol del usuario no tiene permiso para ver ese módulo,
    // así el número ni siquiera se calcula (la validación ocurre en el backend, no solo en la vista).
    public class DashboardDTO
    {
        // Publicaciones en estado Disponible
        public int? AvailablePublications { get; set; }

        // Motos registradas en el sistema
        public int? RegisteredMotorcycles { get; set; }

        // Interesados que todavía no han sido atendidos
        public int? PendingInterests { get; set; }

        // Reservas en estado Vigente
        public int? ActiveReservations { get; set; }

        public bool HasAnyCounter =>
            AvailablePublications.HasValue ||
            RegisteredMotorcycles.HasValue ||
            PendingInterests.HasValue ||
            ActiveReservations.HasValue;
    }
}
