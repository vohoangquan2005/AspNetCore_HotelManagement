using HotelManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.Payment
{
    public class PaymentCreateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã hóa đơn!")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã hóa đơn không hợp lệ!")]
        public int InvoiceId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số tiền thanh toán!")]
        [Range(0.1, Double.MaxValue, ErrorMessage = "Số tiền thanh toán không hợp lệ!")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập loại thanh toán!")]
        public PaymentTypeEnums PaymentType { get; set; } // "Tiền đặt cọc" "Thanh toán lần cuối" "Tiền hoàn trả"
        [Required(ErrorMessage = "Vui lòng nhập phương thức thanh toán!")]
        [StringLength(30, ErrorMessage = "Phương thức thanh toán không vượt quá 30 ký tự!")]
        public string? PaymentMethod { get; set; }
        [StringLength(100, ErrorMessage = "Mã giao dịch không vượt quá 100 ký tự!")]
        public string? TransactionCode { get; set; }
        public PaymentEnums Status { get; set; } // "Đang xử lý" "Thành công" "Thất bại"
        [Required(ErrorMessage = "Vui lòng nhập mã người tạo thanh toán!")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã người tạo thanh toán không hợp lệ!")]
        public int? CreatedByUserId { get; set; }   // Người tạo thanh toán
        public DateTime PaymentDate { get; set; } = DateTime.Now.Date;
    }
}
