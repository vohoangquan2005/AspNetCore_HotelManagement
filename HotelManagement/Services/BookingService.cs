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
                CustomerId = booking.CustomerId,
                BookingDate = booking.BookingDate,
                ExpectedCheckIn = booking.ExpectedCheckIn,
                ExpectedCheckOut = booking.ExpectedCheckOut,
                GuestCount = booking.GuestCount,
                RoomCount = booking.RoomCount,
                PromotionId = booking.PromotionId,
                ServicePackageId = booking.ServicePackageId,
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
                RoomAmount = dto.RoomAmount,
                PackageAmount = dto.PackageAmount,
                DiscountAmount = dto.DiscountAmount,
                AdditionalServiceAmount = dto.AdditionalServiceAmount,
                TotalAmount = dto.TotalAmount,
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
            booking.RoomAmount = dto.RoomAmount;
            booking.PackageAmount = dto.PackageAmount;
            booking.DiscountAmount = dto.DiscountAmount;
            booking.AdditionalServiceAmount = dto.AdditionalServiceAmount;
            booking.TotalAmount = dto.TotalAmount;
            booking.Note = dto.Note;
            booking.Status = dto.Status;
        }
        // Check CustomerId có tồn tại trong bảng Customer không?
        private async Task<bool> CustomerIdExists(int customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
        }
        // Check PromotionId có tồn tại trong bảng Promotion không?
        private async Task<bool> PromotionIdExists(int? promotionId)
        {
            if (promotionId is null) return true;
            return await _context.Promotions.AnyAsync(c => c.PromotionId == promotionId);
        }
        // Check ServicePackageId có tồn tại trong bảng ServicePackage không?
        private async Task<bool> ServicePackageIdExists(int? servicePackageId)
        {
            if (servicePackageId is null) return true;
            return await _context.ServicePackages.AnyAsync(c => c.ServicePackageId == servicePackageId);
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
                CustomerId = b.CustomerId,
                BookingDate = b.BookingDate,
                ExpectedCheckIn = b.ExpectedCheckIn,
                ExpectedCheckOut = b.ExpectedCheckOut,
                GuestCount = b.GuestCount,
                RoomCount = b.RoomCount,
                PromotionId = b.PromotionId,
                ServicePackageId = b.ServicePackageId,
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
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return null;
            return MapToResponseDto(booking);
        }
        // Create 
        public async Task<BookingResponseDto> CreateAsync(BookingCreateDto dto)
        {
            if (!await CustomerIdExists(dto.CustomerId))
                throw new NotFoundException("Mã khách hàng không tồn tại!");
            if (!await PromotionIdExists(dto.PromotionId))
                throw new NotFoundException("Mã khuyến mãi không tồn tại!");
            if (!await ServicePackageIdExists(dto.ServicePackageId))
                throw new NotFoundException("Gói dịch vụ không tồn tại!");
            CheckDate(dto.ExpectedCheckIn, dto.ExpectedCheckOut);
            var booking = MapToEntity(dto);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return MapToResponseDto(booking);
        }
        // Update
        public async Task<BookingResponseDto?> UpdateAsync(int id, BookingUpdateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return null;
            if (!await CustomerIdExists(dto.CustomerId))
                throw new NotFoundException("Mã khách hàng không tồn tại!");
            if (!await PromotionIdExists(dto.PromotionId))
                throw new NotFoundException("Mã khuyến mãi không tồn tại!");
            if (!await ServicePackageIdExists(dto.ServicePackageId))
                throw new NotFoundException("Gói dịch vụ không tồn tại!");
            CheckDate(dto.ExpectedCheckIn, dto.ExpectedCheckOut);
            UpdateEntity(booking, dto);
            await _context.SaveChangesAsync();
            return MapToResponseDto(booking);
        }
        // Delete
        public async Task<BookingResponseDto?> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return null;
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return MapToResponseDto(booking);
        }
    }
}
