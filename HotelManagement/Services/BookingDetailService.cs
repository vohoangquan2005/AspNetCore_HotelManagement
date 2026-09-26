using HotelManagement.Data;
using HotelManagement.DTOs.BookingDetail;
using HotelManagement.Enums;
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
        // Update BookingDetail từ BookingDetailUpdateDto
        private void UpdateEntity(BookingDetail bookingDetail, BookingDetailUpdateDto dto)
        {
            bookingDetail.BookingId = dto.BookingId;
            bookingDetail.RoomId = dto.RoomId;
            bookingDetail.ActualCheckIn = dto.ActualCheckIn;
            bookingDetail.ActualCheckOut = dto.ActualCheckOut;
            bookingDetail.PricePerNight = dto.PricePerNight;
            bookingDetail.GuestCount = dto.GuestCount;
            bookingDetail.TotalAmount = CalculateTotalAmount(dto.ActualCheckIn, dto.ActualCheckOut, dto.PricePerNight);
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
        // Check Date
        private void CheckDate(DateTime? actualCheckIn, DateTime? actualCheckOut)
        {
            if (actualCheckIn is null || actualCheckOut is null) return;
            if (actualCheckIn >= actualCheckOut)
                throw new BadRequestException("Ngày trả phòng phải sau ngày nhận phòng!");
        }
        // Tính tổng tiền TotalAmount
        private decimal CalculateTotalAmount(DateTime? actualCheckIn, DateTime? actualCheckOut, decimal pricePerNight)
        {
            if (actualCheckIn is null || actualCheckOut is null) return 0;
            var numberOfNights = (actualCheckOut.Value.Date - actualCheckIn.Value.Date).Days;
            if (numberOfNights <= 0)
                throw new BadRequestException("Thời gian trả phòng phải sau thời gian nhận phòng!");
            return numberOfNights * pricePerNight;
        }
        // Check phòng đã được check in chưa?
        private bool CheckInExists(DateTime? checkIn)
        {
            return checkIn is not null;
        }
        // Check trạng thái đặt phòng
        private void CheckBookingStatus(BookingEnums status)
        {
            if (status == BookingEnums.Pending)
                throw new BadRequestException("Đặt phòng chưa được xác nhận. Không thể check-in!");
            if (status == BookingEnums.CheckedIn)
                throw new ConflictException("Đặt phòng nãy đã được check-in rồi!");
            if (status == BookingEnums.Cancelled)
                throw new BadRequestException("Đặt phòng đã bị hủy. Không thể check-in!");
            if (status == BookingEnums.Completed)
                throw new BadRequestException("Đặt phòng này đã hoàn thành. Không thể check-in!");
        }
        // Check phòng đã được check out chưa?
        private void CheckOutExists(DateTime? checkOut)
        {
            if (checkOut is not null)
                throw new ConflictException("Phòng nãy đã được check-out rồi!");
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
        // Update
        public async Task<BookingDetailResponseDto?> UpdateAsync(int id, BookingDetailUpdateDto dto)
        {
            var bookingDetail = await _context.BookingDetails.FindAsync(id);
            if (bookingDetail is null) return null;
            await CheckBookingId(dto.BookingId);
            await CheckRoomId(dto.RoomId);
            await CheckBookingRoom(dto.RoomId, dto.BookingId, id);
            await CheckRoomCapacity(dto.RoomId, dto.GuestCount);
            CheckDate(dto.ActualCheckIn, dto.ActualCheckOut);
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
        // Xác nhận CheckIn
        public async Task<bool> CheckInAsync(int id)
        {
            var bookingDetail = await _context.BookingDetails
                                              .Include(bd => bd.Booking)
                                              .FirstOrDefaultAsync(bd => bd.BookingDetailId == id);
            if (bookingDetail is null) return false;
            // Check booking có tồn tại không?
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingDetail.BookingId);
            if (booking is null)
                throw new NotFoundException("Đặt phòng không tồn tại!");
            // Kiểm tra booking đã được xác nhận chưa?
            CheckBookingStatus(bookingDetail.Booking!.Status);
            // Kiểm tra đã được check in chưa
            if (CheckInExists(bookingDetail.ActualCheckIn))
                throw new ConflictException("Phòng này đã được check-in rồi!");
            bookingDetail.ActualCheckIn = DateTime.Now;
            booking.Status = BookingEnums.CheckedIn;
            await _context.SaveChangesAsync();
            return true;
        }
        // Xác nhận CheckOut
        public async Task<bool> CheckOutAsync(int id)
        {
            var bookingDetail = await _context.BookingDetails.FindAsync(id);
            if (bookingDetail is null) return false;
            // Check booking có tồn tại không?
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingDetail.BookingId);
            if (booking is null)
                throw new NotFoundException("Đặt phòng không tồn tại!");
            if (!CheckInExists(bookingDetail.ActualCheckIn))
                throw new ConflictException("Phòng này chưa được check-in!");
            // Kiểm tra booking còn booking detail nào chưa check out không?
            var hasOtherActiveRoom = await _context.BookingDetails
                                                   .AnyAsync(bd => bd.BookingId == booking.BookingId 
                                                                && bd.BookingDetailId != id 
                                                                && bd.ActualCheckIn != null 
                                                                && bd.ActualCheckOut == null);
            CheckOutExists(bookingDetail!.ActualCheckOut);
            bookingDetail.ActualCheckOut = DateTime.Now;
            // Chỉ cập nhật trạng thái Booking thành Completed khi tất cả các booking detail đều đã check out
            if(!hasOtherActiveRoom)
                booking.Status = BookingEnums.Completed;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
