namespace HotelManagement.DTOs.User
{
    public class UserUpdateRequestDto
    {
        public UserUpdateDto User { get; set; } = new();
        public CustomerUpdateDto? Customer { get; set; }
    }
}
