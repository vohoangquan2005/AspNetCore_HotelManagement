using HotelManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.Payment
{
    public class PaymentUpdateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập trạng thái thanh toán!")]
        public PaymentEnums Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        [StringLength(100, ErrorMessage = "Mã giao dịch tối đa 100 ký tự!")]
        public string? TransactionCode { get; set; }
    }
}
