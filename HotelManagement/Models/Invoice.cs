using HotelManagement.Enums;
namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN HÓA ĐƠN
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public decimal SubTotal { get; set; }   // Tiền chưa tính khuyến mãi và thuế
        public decimal TaxAmount { get; set; }  // Tiền thuế (%)
        public decimal DiscountAmount { get; set; } // Số tiền khuyến mãi
        public decimal AmountDue { get; set; }  // Số tiền phải trả
        public InvoiceEnums Status { get; set; }  // "Chưa thanh toán" "Thanh toán 1 phần" "Đã thanh toán"
        public int ? CreatedByUserId { get; set; } // Người tạo hóa đơn
        public DateTime? CreateAt { get; set; }
        // Relationship
        public Booking? Booking { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
