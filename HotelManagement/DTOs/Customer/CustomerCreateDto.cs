using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.Customer
{
    public class CustomerCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Mã User không hợp lệ!")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập CCCD!")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "CCCD phải có 12 ký tự!")]
        public string? CCCD { get; set; }

        [StringLength(50, ErrorMessage = "Quốc tịch không được vượt quá 50 ký tự!")]
        public string? Nationality { get; set; } = "Việt Nam";
    }
}
