using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN HỦY ĐẶT PHÒNG
    public class Cancellation
    {
        public int CancellationId { get; set; }
        public int BookingId { get; set; }
        public int CancellationPolicyId { get; set; }
        public string? Reason { get; set; }
        public decimal RefundPercentage { get; set; }   // Phần trăm hoàn trả
        public decimal RefundAmount { get; set; }   // Số tiền hoàn trả
        public CancellationEnums Status { get; set; }
        public DateTime? CreateAt { get; set; }
        // Relationship
        public Booking? Booking { get; set; }
        public CancellationPolicy? CancellationPolicy { get; set; }
    }
}
