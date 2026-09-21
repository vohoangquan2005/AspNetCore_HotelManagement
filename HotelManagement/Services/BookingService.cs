using HotelManagement.Data;
using HotelManagement.DTOs.Booking;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;
        public BookingService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Booking -> ResponseDto
        private BookingResponseDto MapToResponseDto(Booking booking)
        {
            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                // Thông tin Customer
                CustomerId = booking.CustomerId,
                FullName = booking.Customer?.User?.FullName,
                Email = booking.Customer?.User?.Email,
                PhoneNumber = booking.Customer?.User?.PhoneNumber,
                // Thông tin Booking
                BookingDate = booking.BookingDate,
                ExpectedCheckIn = booking.ExpectedCheckIn,
                ExpectedCheckOut = booking.ExpectedCheckOut,
                GuestCount = booking.GuestCount,
                RoomCount = booking.RoomCount,
                // Thông tin Promotion
                PromotionId = booking.PromotionId,
                PromotionCode = booking.Promotion?.PromotionCode,
                PromotionName = booking.Promotion?.PromotionName,
                DiscountValue = booking.Promotion?.DiscountValue,
                // Thông tin ServicePackage
                ServicePackageId = booking.ServicePackageId,
                PackageName = booking.ServicePackage?.PackageName,
                // Thông tin tiền
                DepositAmount = booking.DepositAmount,
                RoomAmount = booking.RoomAmount,
                PackageAmount = booking.PackageAmount,
                DiscountAmount = booking.DiscountAmount,
                AdditionalServiceAmount = booking.AdditionalServiceAmount,
                TotalAmount = booking.TotalAmount,
                Note = booking.Note,
                Status = booking.Status,
                CreateAt = booking.CreateAt,
                UpdateAt = booking.UpdateAt
            };
        }
        // Chuyển BookingCreateDto -> Booking
        private Booking MapToEntity(BookingCreateDto dto)
        {
            return new Booking
            {
                CustomerId = dto.CustomerId,
                BookingDate = dto.BookingDate,
                ExpectedCheckIn = dto.ExpectedCheckIn,
                ExpectedCheckOut = dto.ExpectedCheckOut,
                GuestCount = dto.GuestCount,
                RoomCount = dto.RoomCount,
                PromotionId = dto.PromotionId,
                ServicePackageId = dto.ServicePackageId,
                DepositAmount = dto.DepositAmount,
                Note = dto.Note
            };
        }
        // Update Booking từ BookingUpdateDto
        private void UpdateEntity(Booking booking, BookingUpdateDto dto)
        {
            booking.CustomerId = dto.CustomerId;
            booking.ExpectedCheckIn = dto.ExpectedCheckIn;
            booking.ExpectedCheckOut = dto.ExpectedCheckOut;
            booking.GuestCount = dto.GuestCount;
            booking.RoomCount = dto.RoomCount;
            booking.PromotionId = dto.PromotionId;
            booking.ServicePackageId = dto.ServicePackageId;
            booking.DepositAmount = dto.DepositAmount;
            booking.Note = dto.Note;
            booking.Status = dto.Status;
        }
        // Check CustomerId có tồn tại trong bảng Customer không?
        private async Task CheckCustomerId(int customerId)
        {
            var exists = await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
            if (!exists)
                throw new NotFoundException("Mã khách hàng không tồn tại!");
        }
        // Check PromotionId có tồn tại trong bảng Promotion không?
        private async Task CheckPromotionId(int? promotionId)
        {
            if (promotionId is null) return;
            var exists = await _context.Promotions.AnyAsync(p => p.PromotionId == promotionId);
            if (!exists)
                throw new NotFoundException("Mã khuyến mãi không tồn tại!");
        }
        // Check ServicePackageId có tồn tại trong bảng ServicePackage không?
        private async Task CheckServicePackageId(int? servicePackageId)
        {
            if (servicePackageId is null) return;
            var exists = await _context.ServicePackages.AnyAsync(sp => sp.ServicePackageId == servicePackageId);
            if (!exists)
                throw new NotFoundException("Gói dịch vụ không tồn tại!");
        }
        // Check Date
        private void CheckDate(DateTime expectedCheckIn, DateTime expectedCheckOut)
        {
            if (expectedCheckIn >= expectedCheckOut)
                throw new BadRequestException("Ngày trả phòng phải sau ngày nhận phòng!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all 
        public async Task<List<BookingResponseDto>> GetAllAsync()
        {
            var bookings = await _context.Bookings.Select(b => new BookingResponseDto
            {
                BookingId = b.BookingId,
                // Thông tin Customer
                CustomerId = b.CustomerId,
                FullName = b.Customer!.User!.FullName,
                Email = b.Customer!.User!.Email,
                PhoneNumber = b.Customer!.User!.PhoneNumber,
                // Thông tin Booking
                BookingDate = b.BookingDate,
                ExpectedCheckIn = b.ExpectedCheckIn,
                ExpectedCheckOut = b.ExpectedCheckOut,
                GuestCount = b.GuestCount,
                RoomCount = b.RoomCount,
                // Thông tin Promotion
                PromotionId = b.PromotionId,
                PromotionCode = b.Promotion != null ? b.Promotion.PromotionCode : null,
                PromotionName = b.Promotion != null ? b.Promotion.PromotionName : null,
                DiscountValue = b.Promotion != null ? b.Promotion.DiscountValue : null,

                // Thông tin ServicePackage
                ServicePackageId = b.ServicePackageId,
                PackageName = b.ServicePackage != null ? b.ServicePackage.PackageName : null,
                // Thông tin tiền
                DepositAmount = b.DepositAmount,
                RoomAmount = b.RoomAmount,
                PackageAmount = b.PackageAmount,
                DiscountAmount = b.DiscountAmount,
                AdditionalServiceAmount = b.AdditionalServiceAmount,
                TotalAmount = b.TotalAmount,
                Note = b.Note,
                Status = b.Status,
                CreateAt = b.CreateAt,
                UpdateAt = b.UpdateAt
            }).ToListAsync();
            return bookings;
        }
        // Get by Id
        public async Task<BookingResponseDto?> GetByIdAsync(int id)
        {
            var booking = await _context.Bookings.Include(b => b.Customer).ThenInclude(c => c!.User)
                                                 .Include(b => b.Promotion)
                                                 .Include(b => b.ServicePackage)
                                                 .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking is null) return null;
            return MapToResponseDto(booking);
        }
        // Create 
        public async Task<BookingResponseDto> CreateAsync(BookingCreateDto dto)
        {
            await CheckCustomerId(dto.CustomerId);
            await CheckPromotionId(dto.PromotionId);
            await CheckServicePackageId(dto.ServicePackageId);
            CheckDate(dto.ExpectedCheckIn, dto.ExpectedCheckOut);
            var booking = MapToEntity(dto);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(booking.BookingId))!;
        }
        // Update
        public async Task<BookingResponseDto?> UpdateAsync(int id, BookingUpdateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return null;
            await CheckCustomerId(dto.CustomerId);
            await CheckPromotionId(dto.PromotionId);
            await CheckServicePackageId(dto.ServicePackageId);
            CheckDate(dto.ExpectedCheckIn, dto.ExpectedCheckOut);
            UpdateEntity(booking, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return false;
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
