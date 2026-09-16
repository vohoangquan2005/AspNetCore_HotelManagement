namespace HotelManagement.Models
{
    public class ApplyPromotion
    {
        public int ApplyPromotionId { get; set; }
        public int PromotionId { get; set; }
        public int RoomTypeId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        // Relationship
        public Promotion? Promotion { get; set; }
        public RoomType? RoomType { get; set; }
    }
}