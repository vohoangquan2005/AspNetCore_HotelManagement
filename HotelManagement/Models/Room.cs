using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN PHÒNG
    public class Room
    {
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public int RoomTypeId { get; set; }
        public int Capacity { get; set; }       // Sức chứa
        public string? Location { get; set; }   // Vị trí: Tầng a khu b
        public decimal PricePerNight { get; set; }// Tiền phòng 1 đêm
        public string? Description { get; set; }
        public RoomEnums Status { get; set; }
        public ActiveEnums IsDeleted { get; set; } // Dùng để xóa tạm
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //Relationship
        public RoomType? RoomType { get; set; }
        public ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<FavoriteRoom> FavoriteRooms { get; set; } = new List<FavoriteRoom>();
    }
}
