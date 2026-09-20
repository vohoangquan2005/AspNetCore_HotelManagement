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
                RoleName = user.Role?.RoleName,
                IsLocked = user.IsLocked,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                IsDeleted = user.IsDeleted,
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
        private async Task CheckRoleId(int roleId)
        {
            var exists = await _context.Roles.AnyAsync(r => r.RoleId == roleId);
            if (!exists)
                throw new NotFoundException("Vai trò không tồn tại!");
        }
        // Check trùng Email
        private async Task EmailExists(string email, int? userId = null)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == email
                                                    &&(userId == null || u.UserId != userId));
            // userId == null để check khi create User
            // u.userId = userId để check khi update User
            if (exists)
                throw new ConflictException("Email đã tồn tại!");
        }
        // Check trùng Số điện thoại
        private async Task PhoneNumberExists(string? phoneNumber, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return;
            var exists = await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber
                                                    &&(userId == null || u.UserId != userId));
            if (exists)
                throw new ConflictException("Số điện thoại đã tồn tại");
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
        // Get all Active(Lấy users còn hoạt động)
        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _context.Users.Where(u => u.IsDeleted == ActiveEnums.Active)
                                            .Select(u => new UserResponseDto
                                            {
                                                UserId = u.UserId,
                                                FullName = u.FullName,
                                                Email = u.Email,
                                                RoleId = u.RoleId,
                                                RoleName = u.Role!.RoleName,
                                                IsLocked = u.IsLocked,
                                                Gender = u.Gender,
                                                DateOfBirth = u.DateOfBirth,
                                                PhoneNumber = u.PhoneNumber,
                                                Avatar = u.Avatar,
                                                IsDeleted = u.IsDeleted,
                                                CreateAt = u.CreateAt,
                                                UpdateAt = u.UpdateAt
                                            }).ToListAsync();
            return users;
        }
        // Get all InActive(Lấy users không hoạt động)
        public async Task<List<UserResponseDto>> GetDeletedAsync()
        {
            var users = await _context.Users.Where(u => u.IsDeleted == ActiveEnums.Inactive)
                                            .Select(u => new UserResponseDto
                                            {
                                                UserId = u.UserId,
                                                FullName = u.FullName,
                                                Email = u.Email,
                                                RoleId = u.RoleId,
                                                RoleName = u.Role!.RoleName,
                                                IsLocked = u.IsLocked,
                                                Gender = u.Gender,
                                                DateOfBirth = u.DateOfBirth,
                                                PhoneNumber = u.PhoneNumber,
                                                Avatar = u.Avatar,
                                                IsDeleted = u.IsDeleted,
                                                CreateAt = u.CreateAt,
                                                UpdateAt = u.UpdateAt
                                            }).ToListAsync();
            return users;
        }
        // Get by Id
        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.UserId == id);
            if (user is null)   return null;
            return MapToResponseDto(user);
        }
        // Create
        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            await CheckRoleId(dto.RoleId);
            await EmailExists(dto.Email!); // ! dùng để chắc chắn rằng email không null cho compiler đừng cảnh báo
            await PhoneNumberExists(dto.PhoneNumber);
            CheckDateOfBirth(dto.DateOfBirth);
            var user = MapToEntity(dto);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(user.UserId))!;
        }
        // Update
        public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return null;
            await CheckRoleId(dto.RoleId);
            await EmailExists(dto.Email!, id);
            await PhoneNumberExists(dto.PhoneNumber, id);
            CheckDateOfBirth(dto.DateOfBirth);
            UpdateEntity(user, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        // Xóa tạm
        public async Task<UserResponseDto?> SoftDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return null;
            user.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Khôi phục (Restore)
        public async Task<UserResponseDto?> RestoreAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return null;
            user.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
    }
}
