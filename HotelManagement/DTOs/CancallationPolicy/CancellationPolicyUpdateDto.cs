using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.CancallationPolicy
{
    public class CancellationPolicyUpdateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên chính sách!")]
        [StringLength(100, ErrorMessage = "Tên chính sách phải ít hơn 100 ký tự!")]
        public string? PolicyName { get; set; }
        public int CancellationHours { get; set; } = 24;    // Số giờ được hủy sau khi đặt phòng
        [Required(ErrorMessage = "Vui lòng nhập phần trăm (%) hoàn trả!")]
        public decimal RefundPercentage { get; set; }   // Phần trăm hoàn trả
        [StringLength(500, ErrorMessage = "Mô tả không vượt quá 500 ký tự!")]
        public string? Description { get; set; }
    }
}
