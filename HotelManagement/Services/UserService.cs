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
        // ===========================================================
        // =                         VALIDATE                        =
        // ===========================================================
        // Lấy Role đang Active
        private async Task<Role> GetActiveRole(int roleId)
        {
            var role = await _context.Roles
                                     .FirstOrDefaultAsync(r => r.RoleId == roleId &&
                                                               r.IsDeleted == ActiveEnums.Active);
            if (role is null)
                throw new NotFoundException("Vai trò không tồn tại hoặc đã bị xóa!");
            return role;
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
        // Check CCCD
        private void CheckCCCD(string? cccd)
        {
            if (cccd is not null && !cccd.All(char.IsDigit))
                throw new BadRequestException("CCCD phải là số!");
        }
        // Check trùng CCCD
        private async Task CCCDExists(string? cccd, int? customerId = null)
        {
            if (string.IsNullOrWhiteSpace(cccd)) return;
            var exists = await _context.Customers
                                       .AnyAsync(c => c.CCCD == cccd 
                                                  && (customerId == null || customerId != c.CustomerId));
            if (exists)
                throw new ConflictException("CCCD đã tồn tại!");
        }
        // ===========================================================
        // =                       CUSTOMER                          =
        // ===========================================================
        // Create Customer
        private async Task CreateCustomer(int userId, CustomerCreateDto dto)
        {
            CheckCCCD(dto.CCCD);
            await CCCDExists(dto.CCCD);
            var customer = new Customer
            {
                UserId = userId,
                CCCD = dto.CCCD,
                Nationality = dto.Nationality,
            };
            await _context.Customers.AddAsync(customer);
        }
        // Update Customer
        private async Task UpdateCustomer(int userId, CustomerUpdateDto dto)
        {
            CheckCCCD(dto.CCCD);
            var customer = await _context.Customers
                                         .FirstOrDefaultAsync(c => c.UserId == userId);
            // User chưa có Customer -> tạo mới
            if (customer is null)
            {
                await CCCDExists(dto.CCCD);
                customer = new Customer
                {
                    UserId = userId,
                    CCCD = dto.CCCD,
                    Nationality = dto.Nationality
                };
                await _context.Customers.AddAsync(customer);
                return;
            }
            // User đã có Customer -> cập nhật
            await CCCDExists(dto.CCCD, customer.CustomerId);
            customer.CCCD = dto.CCCD;
            customer.Nationality = dto.Nationality;
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all Active(Lấy users còn hoạt động)
        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _context.Users.Where(u => u.IsDeleted == ActiveEnums.Active)
                                            .OrderBy(u => u.Role!.RoleName)
                                                .ThenBy(u => u.FullName)
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
                                            .OrderBy(u => u.Role!.RoleName)
                                                .ThenBy(u => u.FullName)
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
        // Get by Id (Chỉ lấy những User còn hoạt động
        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.UserId == id
                                                                  && u.IsDeleted == ActiveEnums.Active);
            if (user is null)   return null;
            return MapToResponseDto(user);
        }
        // Create
        public async Task<UserResponseDto> CreateAsync(UserCreateDto userCreateDto, CustomerCreateDto? customerCreateDto = null)
        {
            await using var transaction = _context.Database.BeginTransaction();
            try
            {
                var role = await GetActiveRole(userCreateDto.RoleId);
                await EmailExists(userCreateDto.Email!); // ! dùng để chắc chắn rằng email không null cho compiler đừng cảnh báo
                await PhoneNumberExists(userCreateDto.PhoneNumber);
                CheckDateOfBirth(userCreateDto.DateOfBirth);
                var user = MapToEntity(userCreateDto);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                // Chỉ tạo Customer khi Role là khách hàng
                if (role.RoleName == "Customer")
                {
                    if (customerCreateDto is null)
                        throw new BadRequestException(
                            "Vui lòng cung cấp thông tin khách hàng!");
                    await CreateCustomer(user.UserId, customerCreateDto);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (await GetByIdAsync(user.UserId))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }
        // Update
        public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto userDto, CustomerUpdateDto? customerUpdateDto = null)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                                         .FirstOrDefaultAsync(u => u.UserId == id 
                                                                && u.IsDeleted == ActiveEnums.Active);
                if (user is null) return null;
                // Validate User
                await EmailExists(userDto.Email!, id);
                await PhoneNumberExists(userDto.PhoneNumber, id);
                CheckDateOfBirth(userDto.DateOfBirth);
                // Lấy Role mới
                var newRole = await GetActiveRole(userDto.RoleId);
                // Role mới là Customer
                if (newRole.RoleName == "Customer")
                {
                    if (customerUpdateDto is null)
                        throw new BadRequestException("Vui lòng cung cấp thông tin khách hàng!");
                    await UpdateCustomer(id, customerUpdateDto);
                }
                // Nếu Role mới không phải Customer:
                // Cập nhật User
                UpdateEntity(user, userDto);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return await GetByIdAsync(id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.UserId == id
                                                             &&u.IsDeleted == ActiveEnums.Inactive);    
            if (user is null) return false;
            var customer = await _context.Customers.AnyAsync(c => c.UserId == id);
            if (customer)
                throw new BadRequestException("Người dùng đang có thông tin khách hàng. Không thể xóa!");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        // Xóa tạm
        public async Task<UserResponseDto?> SoftDeleteAsync(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.UserId == id
                                                             && u.IsDeleted == ActiveEnums.Active);
            if (user is null) return null;
            user.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return MapToResponseDto(user);
        }
        // Khôi phục (Restore)
        public async Task<UserResponseDto?> RestoreAsync(int id)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.UserId == id
                                                             && u.IsDeleted == ActiveEnums.Inactive);
            if (user is null) return null;
            user.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
    }
}
