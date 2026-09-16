namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN PHÒNG YÊU THÍCH (ĐÁNH DẤU ĐỂ XEM SAU)
    public class FavoriteRoom
    {
        public int FavoriteRoomId { get; set; }
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public DateTime? CreateAt { get; set; }
        // Relationship
        public Customer? Customer { get; set; }
        public Room? Room { get; set; }
    }
}
