namespace HotelManagement.DTOs.ApplyPromotion
{
    public class ApplyPromotionResponseDto
    {
        public int ApplyPromotionId { get; set; }
        public int PromotionId { get; set; }
        public int RoomTypeId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
