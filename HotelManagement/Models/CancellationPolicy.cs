using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN CHÍNH SÁCH HỦY ĐẶT PHÒNG
    public class CancellationPolicy
    {
        public int CancellationPolicyId { get; set; }
        public string? PolicyName { get; set; }
        public int CancellationTime { get; set; } = 24;  // Thời gian áp dụng chính sách
        public CancellationTimeUnitEnums CancellationTimeUnit { get; set; } // Đơn vị thời gian
        public decimal RefundPercentage { get; set; }   // Phần trăm hoàn trả
        public string? Description { get; set; }
        public ActiveEnums IsDeleted { get; set; } // Dùng để xóa tạm
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        // Relationship
        public ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
    }
}