namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU CHI TIẾT ĐẶT PHÒNG
    public class BookingDetail
    {
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public DateTime? ActualCheckIn { get; set; } // Thời gian nhận phòng thực tế
        public DateTime? ActualCheckOut { get; set; }// Thời gian trả phòng thực tế
        public decimal PricePerNight { get; set; }
        public decimal TotalAmount { get; set; }
        public int GuestCount { get; set; }         // Số khách ở
        public DateTime? CreateAt { get; set; }
        //Relationship
        public Booking? Booking { get; set; }
        public Room? Room { get; set; }
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    }
}
