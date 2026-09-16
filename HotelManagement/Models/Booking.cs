using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN ĐẶT PHÒNG
    public class Booking
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public DateTime BookingDate { get; set; }       // Ngày đặt phòng
        public DateTime ExpectedCheckIn { get; set; }   // Ngày nhận phòng dự kiến
        public DateTime ExpectedCheckOut { get; set; }  // Ngày trả phòng dự kiến
        public int GuestCount { get; set; }             // Số lượng khách ở
        public int RoomCount { get; set; }              // Số phòng đã đặt
        public int? PromotionId { get; set; }
        public int? ServicePackageId { get; set; }
        public decimal DepositAmount { get; set; }      // Tiền đặt cọc
        public decimal RoomAmount { get; set; }         // Tiền phòng (PricePerNight * số ngày ở) của tất cả Booking Detail
        public decimal PackageAmount { get; set; }      // Tiền gói dịch vụ
        public decimal DiscountAmount { get; set; }     // Tiền giảm khuyến mãi
        public decimal AdditionalServiceAmount { get; set; }    // Tiền dịch vụ đặt thêm
        public decimal TotalAmount { get; set; }        // Tổng số tiền của tất cả chi tiết phòng
        public string? Note { get; set; }
        public BookingEnums Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //Relationship
        public Customer? Customer { get; set; }
        public Promotion? Promotion { get; set; }
        public ServicePackage? ServicePackage { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();
    }
}
