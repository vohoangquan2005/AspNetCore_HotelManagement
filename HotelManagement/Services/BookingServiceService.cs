using HotelManagement.Data;
using HotelManagement.DTOs.BookingService;
using HotelManagement.Exceptions;
using BookingServiceModel = HotelManagement.Models.BookingService;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class BookingServiceService
    {
        private readonly AppDbContext _context;
        public BookingServiceService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển BookingService -> ResponseDto
        private BookingServiceResponseDto MapToResponseDto(BookingServiceModel bookingService)
        {
            return new BookingServiceResponseDto
            {
                BookingServiceId = bookingService.BookingServiceId,
                BookingId = bookingService.BookingId,
                ServiceId = bookingService.ServiceId,
                ServiceName = bookingService.Service?.ServiceName,
                Quantity = bookingService.Quantity,
                UnitPrice = bookingService.UnitPrice,
                TotalAmount = bookingService.TotalAmount,
                CreateAt = bookingService.CreateAt
            };
        }
        // Chuyển CreateDto -> BookingService
        private BookingServiceModel MapToEntity(BookingServiceCreateDto dto)
        {
            return new BookingServiceModel
            {
                BookingId = dto.BookingId,
                ServiceId = dto.ServiceId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TotalAmount = dto.Quantity * dto.UnitPrice
            };
        }
        // Update BookingService từ UpdateDto
        private void UpdateEntity(BookingServiceModel bookingService, BookingServiceUpdateDto dto)
        {
            bookingService.BookingId = dto.BookingId;
            bookingService.ServiceId = dto.ServiceId;
            bookingService.Quantity = dto.Quantity;
            bookingService.UnitPrice = dto.UnitPrice;
            bookingService.TotalAmount = dto.Quantity * dto.UnitPrice;
        }
        // Check BookingId có tồn tại trong bảng Booking không?
        private async Task CheckBookingId(int bookingId)
        {
            var exists = await _context.Bookings.AnyAsync(b => b.BookingId == bookingId);
            if (!exists)
                throw new NotFoundException("Mã đặt phòng không tồn tại!");
        }
        // Check ServiceId có tồn tại trong bảng Service không?
        private async Task CheckServiceId(int serviceId)
        {
            var exists = await _context.Services.AnyAsync(s => s.ServiceId == serviceId);
            if (!exists)
                throw new NotFoundException("Mã dịch vụ không tồn tại!");
        }
        // Check mã đặt phòng BookingId đã đặt dịch vụ ServiceId chưa?
        private async Task CheckBookingService(int bookingId, int serviceId, int? bookingServiceId = null)
        {
            var exists = await _context.BookingServices.AnyAsync(bs => bs.BookingId == bookingId
                                                                   && bs.ServiceId == serviceId
                                                                   && (bookingServiceId == null || bookingServiceId != bs.BookingServiceId));
            if (exists)
                throw new ConflictException("Dịch vụ đã được thêm vào đặt phòng trước đó rồi!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all 
        public async Task<List<BookingServiceResponseDto>> GetAllAsync()
        {
            var bookingServices = await _context.BookingServices
                                                .Select(bs => new BookingServiceResponseDto
                                                {
                                                    BookingServiceId = bs.BookingServiceId,
                                                    BookingId = bs.BookingId,
                                                    ServiceId = bs.ServiceId,
                                                    ServiceName = bs.Service!.ServiceName,
                                                    Quantity = bs.Quantity,
                                                    UnitPrice = bs.UnitPrice,
                                                    TotalAmount = bs.TotalAmount,
                                                    CreateAt = bs.CreateAt
                                                })
                                                .ToListAsync();
            return bookingServices;
        }
        // Get by Id
        public async Task<BookingServiceResponseDto?> GetByIdAsync(int id)
        {
            var bookingService = await _context.BookingServices
                                               .Include(bs => bs.Service)
                                               .FirstOrDefaultAsync(bs => bs.BookingServiceId == id);
            if (bookingService is null) return null;
            return MapToResponseDto(bookingService);
        }
        // Create
        public async Task<BookingServiceResponseDto> CreateAsync(BookingServiceCreateDto dto)
        {
            await CheckBookingId(dto.BookingId);
            await CheckServiceId(dto.ServiceId);
            await CheckBookingService(dto.BookingId, dto.ServiceId);
            var bookingService = MapToEntity(dto);
            await _context.BookingServices.AddAsync(bookingService);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(bookingService.BookingServiceId))!;
        }
        // Update
        public async Task<BookingServiceResponseDto?> UpdateAsync(int id, BookingServiceUpdateDto dto)
        {
            var bookingService = await _context.BookingServices.FindAsync(id);
            if (bookingService is null) return null;
            await CheckBookingId(dto.BookingId);
            await CheckServiceId(dto.ServiceId);
            await CheckBookingService(dto.BookingId, dto.ServiceId, id);
            UpdateEntity(bookingService, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var bookingService = await _context.BookingServices.FindAsync(id);
            if (bookingService is null) return false;
            _context.BookingServices.Remove(bookingService);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
