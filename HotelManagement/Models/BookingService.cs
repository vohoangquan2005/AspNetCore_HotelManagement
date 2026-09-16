namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN ĐẶT DỊCH VỤ
    public class BookingService
    {
        public int BookingServiceId { get; set; }
        public int BookingId { get; set; }
        public int ServiceId { get; set; }
        public int Quantity { get; set; }           // Số lượng
        public decimal UnitPrice { get; set; }      // Đơn giá
        public decimal TotalAmount { get; set; }    // Thành tiền
        public DateTime? CreateAt { get; set; }
        //Relationship
        public Booking? Booking { get; set; }
        public Service? Service { get; set; }
    }
}
