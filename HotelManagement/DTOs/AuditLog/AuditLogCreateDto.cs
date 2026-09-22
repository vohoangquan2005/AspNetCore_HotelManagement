using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.AuditLog
{
    public class AuditLogCreateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Mã người dùng không hợp lệ!")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập thao tác!")]
        [StringLength(50, ErrorMessage = "Thao tác tối đa 50 ký tự!")]
        public string? Action { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên bảng!")]
        [StringLength(100, ErrorMessage = "Tên bảng tối đa 100 ký tự!")]
        public string? TableName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã bản ghi!")]
        [Range(1, int.MaxValue, ErrorMessage = "Mã bản ghi không hợp lệ!")]
        public int RecordId { get; set; }
        [StringLength(4000, ErrorMessage = "Giá trị cũ tối đa 4000 ký tự!")]
        public string? OldValue { get; set; }
        [StringLength(4000, ErrorMessage = "Giá trị mới tối đa 4000 ký tự!")]
        public string? NewValue { get; set; }
    }
}
