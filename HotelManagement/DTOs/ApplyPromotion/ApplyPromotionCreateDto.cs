using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.ApplyPromotion
{
    public class ApplyPromotionCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Mã khuyến mãi không hợp lệ!")]
        public int PromotionId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Mã loại phòng không hợp lệ!")]
        public int RoomTypeId { get; set; }
    }
}
