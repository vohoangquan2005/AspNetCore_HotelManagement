using HotelManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.Room
{
    public class RoomUpdateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên phòng!")]
        [StringLength(50, MinimumLength = 1)]
        public string? RoomName { get; set; }
        public int RoomTypeId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập sức chứa của phòng!")]
        public int Capacity { get; set; }       // Sức chứa
        [StringLength(100, ErrorMessage = "Địa chỉ chứa tối đa 100 ký tự!")]
        public string? Location { get; set; }   // Vị trí: Tầng a khu b
        [Required(ErrorMessage = "Vui lòng nhập giá tiền 1 đêm!")]
        public decimal PricePerNight { get; set; }// Tiền phòng 1 đêm
        public string? Description { get; set; }
        public RoomEnums Status { get; set; }
    }
}
