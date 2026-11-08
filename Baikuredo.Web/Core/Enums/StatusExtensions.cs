namespace Baikuredo.Web.Core.Enums
{
    // Textos en español y colores de etiqueta para mostrar los estados en pantalla
    public static class StatusExtensions
    {
        public static string ToSpanish(this PublicationStatus status)
        {
            return status switch
            {
                PublicationStatus.Available => "Disponible",
                PublicationStatus.Reserved => "Reservada",
                PublicationStatus.Sold => "Vendida",
                PublicationStatus.Cancelled => "Cancelada",
                _ => status.ToString()
            };
        }

        public static string BadgeClass(this PublicationStatus status)
        {
            return status switch
            {
                PublicationStatus.Available => "bg-success",
                PublicationStatus.Reserved => "bg-warning text-dark",
                PublicationStatus.Sold => "bg-primary",
                _ => "bg-secondary"
            };
        }

        public static string ToSpanish(this ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Active => "Vigente",
                ReservationStatus.Completed => "Completada",
                ReservationStatus.Expired => "Expirada",
                _ => status.ToString()
            };
        }

        public static string BadgeClass(this ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Active => "bg-warning text-dark",
                ReservationStatus.Completed => "bg-success",
                _ => "bg-secondary"
            };
        }
    }
}
