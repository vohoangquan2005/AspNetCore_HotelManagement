using HotelManagement.Data;
using HotelManagement.DTOs.BookingDetail;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class BookingDetailService
    {
        private readonly AppDbContext _context;
        public BookingDetailService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển BookingDetail -> ResponseDto
        private BookingDetailResponseDto MapToResponseDto(BookingDetail bookingDetail)
        {
            return new BookingDetailResponseDto
            {
                BookingDetailId = bookingDetail.BookingDetailId,
                BookingId = bookingDetail.BookingId,
                RoomId = bookingDetail.RoomId,
                RoomName = bookingDetail.Room?.RoomName,
                ActualCheckIn = bookingDetail.ActualCheckIn,
                ActualCheckOut = bookingDetail.ActualCheckOut,
                PricePerNight = bookingDetail.PricePerNight,
                TotalAmount = bookingDetail.TotalAmount,
                GuestCount = bookingDetail.GuestCount,
                CreateAt = bookingDetail.CreateAt
            };
        }
        // Chuyển BookingCreateDto -> BookingDetail
        private BookingDetail MapToEntity(BookingDetailCreateDto dto)
        {
            return new BookingDetail
            {
                BookingId = dto.BookingId,
                RoomId = dto.RoomId,
                ActualCheckIn = dto.ActualCheckIn,
                ActualCheckOut = dto.ActualCheckOut,
                PricePerNight = dto.PricePerNight,
                TotalAmount = dto.TotalAmount,
                GuestCount = dto.GuestCount
            };
        }
        // Update BookingDetail từ BookingDetailUpdateDto
        private void UpdateEntity(BookingDetail bookingDetail, BookingDetailUpdateDto dto)
        {
            bookingDetail.BookingId = dto.BookingId;
            bookingDetail.RoomId = dto.RoomId;
            bookingDetail.ActualCheckIn = dto.ActualCheckIn;
            bookingDetail.ActualCheckOut = dto.ActualCheckOut;
            bookingDetail.PricePerNight = dto.PricePerNight;
            bookingDetail.TotalAmount = dto.TotalAmount;
            bookingDetail.GuestCount = dto.GuestCount;
        }
        // Check BookingId có tồn tại trong bảng Booking không?
        private async Task CheckBookingId(int bookingId)
        {
            var exists = await _context.Bookings.AnyAsync(b => b.BookingId == bookingId);
            if (!exists)
                throw new NotFoundException("Mã đặt phòng không tồn tại!");
        }
        // Check RoomId có tồn tại trong bảng Room không?
        private async Task CheckRoomId(int roomId)
        {
            var exists = await _context.Rooms.AnyAsync(r => r.RoomId == roomId);
            if (!exists)
                throw new NotFoundException("Mã phòng không tồn tại!");
        }
        // Check phòng RoomId đã được BookingId đặt chưa?
        private async Task CheckBookingRoom(int roomId, int bookingId, int? bookingDetailId = null)
        {
            var exists = await _context.BookingDetails.AnyAsync(bd => bd.RoomId == roomId 
                                                                   && bd.BookingId == bookingId
                                                                   && (bookingDetailId == null || bookingDetailId != bd.BookingDetailId));
            if (exists)
                throw new ConflictException("Mã đặt phòng đã đặt phòng này rồi!");
        }
        // Check GuestCount của BookingDetail có vượt quá Capacity của Room không?
        private async Task CheckRoomCapacity(int roomId, int guestCount)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room is null)
                throw new NotFoundException("Phòng không tồn tại!");
            if (guestCount > room.Capacity)
                throw new BadRequestException(
                    "Số lượng khách vượt quá sức chứa của phòng!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all 
        public async Task<List<BookingDetailResponseDto>> GetAllAsync()
        {
            var bookingDetails = await _context.BookingDetails.Select(bd => new BookingDetailResponseDto
            {
                BookingDetailId = bd.BookingDetailId,
                BookingId = bd.BookingId,
                RoomId = bd.RoomId,
                RoomName = bd.Room!.RoomName,
                ActualCheckIn = bd.ActualCheckIn,
                ActualCheckOut = bd.ActualCheckOut,
                PricePerNight = bd.PricePerNight,
                TotalAmount = bd.TotalAmount,
                GuestCount = bd.GuestCount,
                CreateAt = bd.CreateAt
            }).ToListAsync();
            return bookingDetails;
        }
        // Get by Id
        public async Task<BookingDetailResponseDto?> GetByIdAsync(int id)
        {
            var bookingDetail = await _context.BookingDetails.Include(bd => bd.Room)
                                                             .FirstOrDefaultAsync(bd => bd.BookingDetailId == id);
            if (bookingDetail is null) return null;
            return MapToResponseDto(bookingDetail);
        }
        // Create 
        public async Task<BookingDetailResponseDto> CreateAsync(BookingDetailCreateDto dto)
        {
            await CheckBookingId(dto.BookingId);
            await CheckRoomId(dto.RoomId);
            await CheckBookingRoom(dto.RoomId, dto.BookingId);
            await CheckRoomCapacity(dto.RoomId, dto.GuestCount);
            var bookingDetail = MapToEntity(dto);
            await _context.BookingDetails.AddAsync(bookingDetail);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(bookingDetail.BookingDetailId))!;
        }
        // Update
        public async Task<BookingDetailResponseDto?> UpdateAsync(int id, BookingDetailUpdateDto dto)
        {
            var bookingDetail = await _context.BookingDetails.FindAsync(id);
            if (bookingDetail is null) return null;
            await CheckBookingId(dto.BookingId);
            await CheckRoomId(dto.RoomId);
            await CheckBookingRoom(dto.RoomId, dto.BookingId, id);
            await CheckRoomCapacity(dto.RoomId, dto.GuestCount);
            UpdateEntity(bookingDetail, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var bookingDetail = await _context.BookingDetails.FindAsync(id);
            if (bookingDetail is null) return false;
            _context.BookingDetails.Remove(bookingDetail);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
