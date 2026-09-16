namespace HotelManagement.Models
{
    // DÙNG ĐỂ LƯU VAI TRÒ
    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        // Relatonship
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
