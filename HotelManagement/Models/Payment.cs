using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THANH TOÁN
    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentType { get; set; } // "Tiền đặt cọc" "Thanh toán lần cuối" "Tiền hoàn trả"
        public string? PaymentMethod { get; set; }
        public string? TransactionCode { get; set; }
        public PaymentEnums Status { get; set; } // "Đang xử lý" "Thành công" "Thất bại"
        public int? CreatedByUserId { get; set; }   // Người tạo thanh toán
        public DateTime? PaymentDate { get; set; }
        public DateTime? CreateAt { get; set; }
        // Relationship
        public Invoice? Invoice { get; set; }
        public User? CreatedByUser { get; set; }
    }
}