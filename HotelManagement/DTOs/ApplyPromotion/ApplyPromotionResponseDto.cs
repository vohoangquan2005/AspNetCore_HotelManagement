namespace HotelManagement.DTOs.ApplyPromotion
{
    public class ApplyPromotionResponseDto
    {
        public int ApplyPromotionId { get; set; }
        // Thông tin Promotion
        public int PromotionId { get; set; }
        public string? PromotionCode { get; set; }
        public string? PromotionName { get; set; }
        // Thông tin RoomType
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
