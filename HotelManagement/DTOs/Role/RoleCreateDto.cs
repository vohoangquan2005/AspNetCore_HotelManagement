using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs.Role
{
    public class RoleCreateDto
    {
        [StringLength(50, MinimumLength = 1)]
        [Required(ErrorMessage = "Vui lòng nhập tên vai trò!")]
            public string? RoleName { get; set; }
        [StringLength(255)]
            public string? Description { get; set; }
    }
}
