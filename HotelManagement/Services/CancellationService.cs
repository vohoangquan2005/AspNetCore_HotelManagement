using HotelManagement.Data;
using HotelManagement.DTOs.Cancellation;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class CancellationService
    {
        private readonly AppDbContext _context;
        public CancellationService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Cancellation -> ResponseDto
        private CancellationResponseDto MapToResponseDto(Cancellation cancellation)
        {
            return new CancellationResponseDto
            {
                CancellationId = cancellation.CancellationId,
                BookingId = cancellation.BookingId,
                CancellationPolicyId = cancellation.CancellationPolicyId,
                Reason = cancellation.Reason,
                RefundPercentage = cancellation.RefundPercentage,
                RefundAmount = cancellation.RefundAmount,
                Status = cancellation.Status,
                CreateAt = cancellation.CreateAt
            };
        }
        // Chuyển CancellationCreateDto -> Cancellation
        private Cancellation MapToEntity(CancellationCreateDto dto, decimal refundPercentage, decimal refundAmount)
        {
            return new Cancellation
            {
                BookingId = dto.BookingId,
                CancellationPolicyId = dto.CancellationPolicyId,
                Reason = dto.Reason,
                RefundPercentage = refundPercentage,
                RefundAmount = refundAmount,
                Status = CancellationEnums.Pending
            };
        }
        // Check BookingId có tồn tại trong bảng Booking không?
        private async Task<Booking> CheckBookingId(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking is null)
                throw new NotFoundException("Mã đặt phòng không hợp lệ!");
            return booking;
        }
        // Check CancellationPolicyId có tồn tại trong bảng CancellationPolicy không?
        private async Task<CancellationPolicy> CheckCancellationPolicyId(int cancellationPolicyId)
        {
            var policy = await _context.CancellationPolicies
                .FirstOrDefaultAsync(cp => cp.CancellationPolicyId == cancellationPolicyId
                                        && cp.IsDeleted == ActiveEnums.Active);
            if (policy is null)
                throw new NotFoundException("Mã chính sách hủy không hợp lệ hoặc chính sách không còn hoạt động!");
            return policy;
        }
        // Check BookingId đã có yêu cầu hủy phòng chưa?
        private async Task CheckCancellationExists(int bookingId)
        {
            var exists = await _context.Cancellations.AnyAsync(c => c.BookingId == bookingId
                                                                 && c.Status != CancellationEnums.Rejected);
            if (exists)
                throw new ConflictException("Đặt phòng này đã có yêu cầu hủy!");
        }
        // Check Booking có được phép hủy không
        private void CheckBookingStatus(Booking booking)
        {
            if (booking.Status == BookingEnums.Cancelled)
                throw new BadRequestException("Đặt phòng đã được hủy!");
            if (booking.Status == BookingEnums.Completed)
                throw new BadRequestException("Đặt phòng đã hoàn thành, không thể hủy!");
            if (booking.Status == BookingEnums.CheckedIn)
                throw new BadRequestException("Đặt phòng đã nhận phòng, không thể hủy!");
        }
        // Check thời gian hủy
        private void CheckCancellationTime(Booking booking, CancellationPolicy policy)
        {
            var currentTime = DateTime.Now;
            var hours = (booking.ExpectedCheckIn - currentTime).TotalHours;
            if (hours < policy.CancellationHours)
                throw new BadRequestException("Đã quá thời gian cho phép hủy đặt phòng!");
        }
        // Tính số tiền hoàn trả
        private decimal CalculateRefundAmount(decimal depositAmount, decimal refundPercentage)
        {
            return depositAmount * refundPercentage / 100;
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================

        // Get all
        public async Task<List<CancellationResponseDto>> GetAllAsync()
        {
            var cancellations = await _context.Cancellations
                                              .Select(c => new CancellationResponseDto
                                              {
                                                    CancellationId = c.CancellationId,
                                                    BookingId = c.BookingId,
                                                    CancellationPolicyId = c.CancellationPolicyId,
                                                    Reason = c.Reason,
                                                    RefundPercentage = c.RefundPercentage,
                                                    RefundAmount = c.RefundAmount,
                                                    Status = c.Status,
                                                    CreateAt = c.CreateAt
                                              })
                                              .ToListAsync();
            return cancellations;
        }
        // Get by Id
        public async Task<CancellationResponseDto?> GetByIdAsync(int id)
        {
            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation is null) return null;
            return MapToResponseDto(cancellation);
        }
        // Create
        public async Task<CancellationResponseDto> CreateAsync(CancellationCreateDto dto)
        {
            var booking = await CheckBookingId(dto.BookingId);
            var policy = await CheckCancellationPolicyId(dto.CancellationPolicyId);
            await CheckCancellationExists(dto.BookingId);
            CheckBookingStatus(booking);
            CheckCancellationTime(booking, policy);
            var refundAmount = CalculateRefundAmount(booking.DepositAmount, policy.RefundPercentage);
            var cancellation = MapToEntity(dto, policy.RefundPercentage, refundAmount);
            await _context.Cancellations.AddAsync(cancellation);
            await _context.SaveChangesAsync();

            return MapToResponseDto(cancellation);
        }
        // Update
        public async Task<CancellationResponseDto?> UpdateAsync(int id, CancellationUpdateDto dto)
        {
            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation is null) return null;
            if (cancellation.Status != CancellationEnums.Pending)
                throw new BadRequestException("Chỉ có thể cập nhật yêu cầu hủy đang chờ xử lý!");
            cancellation.Reason = dto.Reason;
            await _context.SaveChangesAsync();
            return MapToResponseDto(cancellation);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var cancellation = await _context.Cancellations.FindAsync(id);
            if (cancellation is null) return false;
            if (cancellation.Status != CancellationEnums.Pending)
                throw new BadRequestException("Chỉ có thể xóa yêu cầu hủy đang chờ xử lý!");
            _context.Cancellations.Remove(cancellation);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
