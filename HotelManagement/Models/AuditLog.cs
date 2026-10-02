using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU NHẬT KÝ THAO TÁC TRÊN DATABASE
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int? UserId { get; set; }
        public AuditLogEnums Action { get; set; }  
        public string? TableName { get; set; }
        public int RecordId { get; set; }    // ID bản ghi bị tác động
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime? CreateAt { get; set; }
        // Relationship
        public User? User { get; set; }
    }
}
