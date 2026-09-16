namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN KHÁCH HÀNG ĐẶT PHÒNG
    public class Customer
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string? CCCD { get; set; }
        public string? Nationality { get; set; } = "Việt Nam";  // Quốc tịch
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //Relationship
        public User? User { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<FavoriteRoom> FavoriteRooms { get; set; } = new List<FavoriteRoom>();
    }
}
