using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.Booking
{
    public class BookingCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Mã khách hàng không hợp lệ!")]
        public int CustomerId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ExpectedCheckIn { get; set; }
        public DateTime ExpectedCheckOut { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng khách phải lớn hơn 0!")]
        public int GuestCount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phòng phải lớn hơn 0!")]
        public int RoomCount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Mã khuyến mãi không hợp lệ!")]
        public int? PromotionId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Mã gói dịch vụ không hợp lệ!")]
        public int? ServicePackageId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tiền đặt cọc không được âm!")]
        public decimal DepositAmount { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tiền phòng không được âm!")]
        public decimal RoomAmount { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tiền gói dịch vụ không được âm!")]
        public decimal PackageAmount { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tiền giảm giá không được âm!")]
        public decimal DiscountAmount { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tiền dịch vụ thêm không được âm!")]
        public decimal AdditionalServiceAmount { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không được âm!")]
        public decimal TotalAmount { get; set; }
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự!")]
        public string? Note { get; set; }
    }
}
