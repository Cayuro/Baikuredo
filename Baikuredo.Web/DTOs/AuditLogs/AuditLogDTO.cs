namespace Baikuredo.Web.DTOs.AuditLogs
{
    public class AuditLogDTO
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public Guid RecordId { get; set; }
        public string? Detail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
