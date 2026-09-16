using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU LOẠI PHÒNG
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public int BedCount { get; set; }
        public string? BedType { get; set; }
        public string? Size { get; set; }           // Kích thước dài x rộng
        public string? Description { get; set; }
        public ActiveEnums IsDeleted { get; set; } // Dùng để xóa tạm
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //Relationship
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<ApplyPromotion> ApplyPromotions { get; set; } = new List<ApplyPromotion>();
    }
}
