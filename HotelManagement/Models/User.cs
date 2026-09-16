using HotelManagement.Enums;
namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU NGƯỜI DÙNG
    public class User
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? HashPassword { get; set; }
        public int RoleId { get; set; }
        public ActiveEnums IsLocked {get; set; }         // Active or Inactive
        public string? Gender { get; set; }     // Giới tính
        public DateTime? DateOfBirth { get; set; } // Ngày sinh
        public string? PhoneNumber { get; set; }      // Số điện thoại
        public string? Avatar { get; set; }     // Ảnh đại diện
        public ActiveEnums IsDeleted { get; set; }  // Người dùng có bị xóa không(Dùng để xóa tạm)
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        // Relation ship
        public Role? Role { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<Invoice> CreatedInvoices { get; set; } = new List<Invoice>();
        public ICollection<Payment> CreatedPayments { get; set; } = new List<Payment>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
