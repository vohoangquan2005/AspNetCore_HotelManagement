using HotelManagement.Enums;

namespace HotelManagement.DTOs.Invoice
{
    public class InvoiceResponseDto
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public string? CustomerName { get; set; }
        public decimal SubTotal { get; set; }   // Tiền chưa tính khuyến mãi và thuế
        public decimal TaxAmount { get; set; }  // Tiền thuế
        public decimal DiscountAmount { get; set; } // Số tiền khuyến mãi
        public decimal AmountDue { get; set; }  // Số tiền phải trả
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; } // Số tiền chưa thanh toán
        public InvoiceEnums Status { get; set; }  // "Chưa thanh toán" "Thanh toán 1 phần" "Đã thanh toán"
        public int? CreatedByUserId { get; set; } // Người tạo hóa đơn
        public DateTime? CreateAt { get; set; }
    }
}
