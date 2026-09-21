using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.BookingService
{
    public class BookingServiceCreateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã đặt phòng!")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã đặt phòng không hợp lệ!")]
        public int BookingId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã dịch vụ!")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã  dịch vụ không hợp lệ!")]
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng!")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0!")]
        public int Quantity { get; set; }           // Số lượng
        [Required(ErrorMessage = "Vui lòng nhập đơn giá!")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không hợp lệ!")]
        public decimal UnitPrice { get; set; }      // Đơn giá
    }
}
