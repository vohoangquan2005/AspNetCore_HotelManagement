using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN CHÍNH SÁCH HỦY ĐẶT PHÒNG
    public class CancellationPolicy
    {
        public int CancellationPolicyId { get; set; }
        public string? PolicyName { get; set; }
        public int CancellationHours { get; set; } = 24;    // Số giờ được hủy sau khi đặt phòng
        public decimal RefundPercentage { get; set; }   // Phần trăm hoàn trả
        public string? Description { get; set; }
        public ActiveEnums IsDeleted { get; set; } // Dùng để xóa tạm
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        // Relationship
        public ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
    }
}