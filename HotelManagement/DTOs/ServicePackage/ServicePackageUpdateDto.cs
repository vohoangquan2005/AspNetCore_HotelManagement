using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.ServicePackage
{
    public class ServicePackageUpdateDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên gói dịch vụ!")]
        [StringLength(100)]
        public string? PackageName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá tiền gói dịch vụ!")]
        public decimal PackagePrice { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
    }
}
