using HotelManagement.Data;
using HotelManagement.DTOs.User;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Enums;

namespace HotelManagement.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển User -> UserResponseDto
        private UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                IsLocked = user.IsLocked,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt
            };
        }
        // Chuyển UserCreateDto -> User
        private User MapToEntity(UserCreateDto dto)
        {
            return new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                HashPassword = dto.HashPassword,
                RoleId = dto.RoleId,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                Avatar = dto.Avatar
            };
        }
        // Cập nhật User từ UserUpdateDto
        private void UpdateEntity(User user, UserUpdateDto dto)
        {
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.HashPassword = dto.HashPassword;
            user.RoleId = dto.RoleId;
            user.IsLocked = dto.IsLocked;
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.PhoneNumber = dto.PhoneNumber;
            user.Avatar = dto.Avatar;
        }
        // Check RoleId đã được tạo trong bảng Roles chưa?
        private async Task<bool> RoleIdExistsAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.RoleId == roleId);
        }
        // Check trùng Email
        private async Task<bool> EmailExistsAsync(string email, int? userId = null)
        {
            return await _context.Users.AnyAsync(u => u.Email == email
                                                    &&(userId == null || u.UserId != userId));
            // userId == null để check khi create User
            // u.userId = userId để check khi update User
        }
        // Check trùng Số điện thoại
        private async Task<bool> PhoneNumberExistsAsync(string? phoneNumber, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))  return false;
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber
                                                    &&(userId == null || u.UserId != userId));
        }
        // Check ngày sinh
        private void CheckDateOfBirth(DateTime? dateOfBirth)
        {
            if (dateOfBirth.HasValue && dateOfBirth.Value.Date > DateTime.Today)
            {
                throw new BadRequestException("Ngày sinh không được lớn hơn ngày hiện tại!");
            }
        }    
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all
        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _context.Users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                RoleId = u.RoleId,
                IsLocked = u.IsLocked,
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                CreateAt = u.CreateAt,
                UpdateAt = u.UpdateAt
            }).ToListAsync();
            return users;
        }
        // Get by Id
        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)   return null;
            return MapToResponseDto(user);
        }
        // Create
        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            if (!await RoleIdExistsAsync(dto.RoleId))
                throw new NotFoundException("Role không tồn tại!");
            if (await EmailExistsAsync(dto.Email!)) // ! dùng để chắc chắn rằng email không null cho compiler đừng cảnh báo
                throw new ConflictException("Email đã tồn tại!");
            if (await PhoneNumberExistsAsync(dto.PhoneNumber))
                throw new ConflictException("Số điện thoại đã tồn tại");
            CheckDateOfBirth(dto.DateOfBirth);
            var user = MapToEntity(dto);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return MapToResponseDto(user);
        }
        // Update
        public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return null;
            if (!await RoleIdExistsAsync(dto.RoleId))
                throw new NotFoundException("Role không tồn tại!");
            if (await EmailExistsAsync(dto.Email!, id))
                throw new ConflictException("Email đã tồn tại!");
            if (await PhoneNumberExistsAsync(dto.PhoneNumber, id))
                throw new ConflictException("Số điện thoại đã tồn tại");
            CheckDateOfBirth(dto.DateOfBirth);
            UpdateEntity(user, dto);
            await _context.SaveChangesAsync();
            return MapToResponseDto(user);
        }
        // Delete
        public async Task<UserResponseDto?> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return null;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return MapToResponseDto(user);
        }
        // Xóa tạm
        public async Task<UserResponseDto?> SoftDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return null;
            user.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return MapToResponseDto(user);
        }
        // Khôi phục (Restore)
        public async Task<UserResponseDto?> RestoreAsync(int id)
        {
            var user = await _context.Users.IgnoreQueryFilters()
                                           .FirstOrDefaultAsync(u => u.UserId == id);
            if (user is null) return null;
            user.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return MapToResponseDto(user);
        }
    }
}
