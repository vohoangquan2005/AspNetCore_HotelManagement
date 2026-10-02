using HotelManagement.Enums;

namespace HotelManagement.DTOs.AuditLog
{
    public class AuditLogResponseDto
    {
        public int AuditLogId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public AuditLogEnums Action { get; set; }
        public string? TableName { get; set; }
        public int RecordId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
