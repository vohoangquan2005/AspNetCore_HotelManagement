using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN ĐÁNH GIÁ PHÒNG
    public class Review
    {
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public int BookingId { get; set; }
        public int Rating { get; set; }     // Số sao
        public string? Comment { get; set; }
        public ReviewEnums Status { get; set; } // "Chờ xử lý" "Đã duyệt" "Từ chối"
        public DateTime? CreateAt { get; set; }
        // Relationship
        public Customer? Customer { get; set; }
        public Room? Room { get; set; }
        public Booking? Booking { get; set; }
    }
}
