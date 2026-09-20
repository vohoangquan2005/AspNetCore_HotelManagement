using HotelManagement.Data;
using HotelManagement.DTOs.Customer;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class CustomerService
    {
        private readonly AppDbContext _context;
        public CustomerService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Customer -> ResponseDto
        private CustomerResponseDto MapToResponseDto(Customer customer)
        {
            return new CustomerResponseDto
            {
                CustomerId = customer.CustomerId,
                UserId = customer.UserId,
                CCCD = customer.CCCD,
                Nationality = customer.Nationality,
                FullName = customer.User?.FullName,
                Email = customer.User?.Email,
                Gender = customer.User?.Gender,
                DateOfBirth = customer.User?.DateOfBirth,
                PhoneNumber = customer.User?.PhoneNumber,
                Avatar = customer.User?.Avatar,
                CreateAt = customer.CreateAt,
                UpdateAt = customer.UpdateAt
            };
        }
        // Chuyển CreateDto -> Customer
        private Customer MapToEntity(CustomerCreateDto dto)
        {
            return new Customer
            {
                UserId = dto.UserId,
                CCCD = dto.CCCD,
                Nationality = dto.Nationality
            };
        }
        // Cập nhật Customer từ UpdateDto
        private void UpdateEntity(Customer customer, CustomerUpdateDto dto)
        {
            customer.UserId = dto.UserId;
            customer.CCCD = dto.CCCD;
            customer.Nationality = dto.Nationality;
        }
        // Check UserId có tồn tại trong bảng User không?
        private async Task CheckUserId(int userId)
        {
            var exists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!exists)
                throw new BadRequestException("Mã người dùng không tồn tại!");
        }
        // Check trùng UserId
        private async Task UserIdExists(int userId, int? customerId = null)
        {
            var exists = await _context.Customers.AnyAsync(c => c.UserId == userId
                                                 && (customerId == null || customerId != c.CustomerId));
            if (exists)
                throw new ConflictException("Mã người dùng đã được sử dụng cho khách hàng khác!");
        }
        // Check CCCD
        private async Task CheckCCCD(string cccd, int? customerId = null)
        {
            var exists = await _context.Customers.AnyAsync(c => c.CCCD == cccd
                                                  && (customerId == null || customerId != c.CustomerId));
            if (exists)
                throw new ConflictException("CCCD đã tồn tại!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all
        public async Task<List<CustomerResponseDto>> GetAllAsync() 
        {
            var customers = await _context.Customers.Select(c => new CustomerResponseDto
                                                    {
                                                        CustomerId = c.CustomerId,
                                                        UserId = c.UserId,
                                                        CCCD = c.CCCD,
                                                        Nationality = c.Nationality,
                                                        FullName = c.User!.FullName,
                                                        Email = c.User.Email,
                                                        Gender = c.User.Gender,
                                                        DateOfBirth = c.User.DateOfBirth,
                                                        PhoneNumber = c.User.PhoneNumber,
                                                        Avatar = c.User.Avatar,
                                                        CreateAt = c.CreateAt,
                                                        UpdateAt = c.UpdateAt
                                                    }).ToListAsync();
            return customers;
        }
        // Get by Id
        public async Task<CustomerResponseDto?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.Include(c => c.User)
                                                   .FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null) return null;
            return MapToResponseDto(customer);
        }
        // Create
        public async Task<CustomerResponseDto> CreateAsync(CustomerCreateDto dto)
        {
            await CheckUserId(dto.UserId);
            await UserIdExists(dto.UserId);
            await CheckCCCD(dto.CCCD!);
            var customer = MapToEntity(dto);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(customer.CustomerId))!;
        }
        // Update
        public async Task<CustomerResponseDto?> UpdateAsync(int id, CustomerUpdateDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null) return null;
            await CheckUserId(dto.UserId);
            await UserIdExists(dto.UserId, id);
            await CheckCCCD(dto.CCCD!, id);
            UpdateEntity(customer, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers.Include(c => c.User)
                                                   .FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null) return false;
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
