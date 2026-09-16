namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU NHẬT KÝ THAO TÁC TRÊN DATABASE
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int? UserId { get; set; }
        public string? Action { get; set; }  // "Thêm" "Sửa" "Xóa" "Đăng nhập" "Đăng xuất"
        public string? TableName { get; set; }
        public int RecordId { get; set; }    // Bản ghi được thay đổi
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime? CreateAt { get; set; }
        // Relationship
        public User? User { get; set; }
    }
}
