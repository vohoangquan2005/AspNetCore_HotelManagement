using HotelManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.RoomType
{
    public class RoomTypeCreateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên loại phòng!")]
        [StringLength(50, ErrorMessage = "Tên loại phòng không vượt quá 50 ký tự!")]
        public string? RoomTypeName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng giường!")]
        public int BedCount { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập loại giường!")]
        public string? BedType { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập kích thước phòng!")]
        public string? Size { get; set; }           // Kích thước dài x rộng
        public string? Description { get; set; }
    }
}
