using HotelManagement.Enums;
namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN DỊCH VỤ
    public class Service
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public decimal UnitPrice { get; set; }      // Đơn giá
        public string? Unit { get; set; }   // Đơn vị tính (Theo giờ, theo ngày, theo lần)
        public string? Description { get; set; }
        public ActiveEnums Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //Relationship
        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
