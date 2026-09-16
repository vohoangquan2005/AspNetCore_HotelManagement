using HotelManagement.Enums;

namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU THÔNG TIN GÓI DỊCH VỤ
    public class ServicePackage
    {
        public int ServicePackageId { get; set; }
        public string? PackageName { get; set; }
        public decimal PackagePrice { get; set; }
        public string? Description { get; set; }
        public ActiveEnums IsDeleted { get; set; } // Dùng để xóa tạm
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        //Relationship
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
