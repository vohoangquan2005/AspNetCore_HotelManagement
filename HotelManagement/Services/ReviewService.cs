using HotelManagement.Data;
using HotelManagement.DTOs.Review;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class ReviewService
    {
        private readonly AppDbContext _context;
        public ReviewService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Review -> ResponseDto
        private ReviewResponseDto MapToResponseDto(Review review)
        {
            return new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                CustomerId = review.CustomerId,
                CustomerName = review.Customer?.User?.FullName,
                RoomId = review.RoomId,
                RoomName = review.Room?.RoomName,
                BookingId = review.BookingId,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreateAt = review.CreateAt
            };
        }
        // Chuyển ReviewCreateDto -> Review
        private Review MapToEntity(ReviewCreateDto dto)
        {
            return new Review
            {
                CustomerId = dto.CustomerId,
                RoomId = dto.RoomId,
                BookingId = dto.BookingId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                Status = ReviewEnums.Pending
            };
        }
        // Update Review từ UpdateDto
        private void UpdateEntity(Review review, ReviewUpdateDto dto)
        {
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
        }
        // Check CustomerId có tồn tại trong bảng Customer không?
        private async Task CheckCustomerId(int customerId)
        {
            var exists = await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
            if (!exists)
                throw new NotFoundException("Mã khách hàng không hợp lệ!");
        }
        // Check RoomId có tồn tại trong bảng Room không
        private async Task CheckRoomId(int roomId)
        {
            var exists = await _context.Rooms.AnyAsync(r => r.RoomId == roomId);
            if (!exists)
                throw new NotFoundException("Mã phòng không hợp lệ!");
        }
        // Check BookingId có tồn tại trong bảng Booking không
        private async Task CheckBookingId(int bookingId)
        {
            var exists = await _context.Bookings.AnyAsync(b => b.BookingId == bookingId);
            if (!exists)
                throw new NotFoundException("Mã đặt phòng không hợp lệ!");
        }
        // Check Customer + Room + Booking có liên quan với nhau không
        private async Task CheckCustomerBookingRoom(int customerId, int roomId, int bookingId)
        {
            var exists = await _context.Bookings
                                       .AnyAsync(b => b.BookingId == bookingId 
                                                   && b.CustomerId == customerId 
                                                   && b.BookingDetails.Any(bd => bd.RoomId == roomId));
            if (!exists)
                throw new BadRequestException("Khách hàng, phòng và đặt phòng không hợp lệ!");
        }
        // Check đã đánh giá phòng trong Booking này chưa
        private async Task CheckReviewExists(int customerId, int roomId, int bookingId)
        {
            var exists = await _context.Reviews.AnyAsync(r =>r.CustomerId == customerId 
                                                          && r.RoomId == roomId 
                                                          && r.BookingId == bookingId);
            if (exists)
                throw new ConflictException("Khách hàng đã đánh giá phòng này trong đặt phòng!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all
        public async Task<List<ReviewResponseDto>> GetAllAsync()
        {
            var reviews = await _context.Reviews
                                        .Include(r => r.Customer)
                                            .ThenInclude(c => c!.User)
                                        .Include(r => r.Room)
                                        .Select(r => new ReviewResponseDto
                                        {
                                            ReviewId = r.ReviewId,
                                            CustomerId = r.CustomerId,
                                            CustomerName = r.Customer!.User!.FullName,
                                            RoomId = r.RoomId,
                                            RoomName = r.Room!.RoomName,
                                            BookingId = r.BookingId,
                                            Rating = r.Rating,
                                            Comment = r.Comment,
                                            Status = r.Status,
                                            CreateAt = r.CreateAt
                                        })
                                        .ToListAsync();
            return reviews;
        }
        // Get by Id
        public async Task<ReviewResponseDto?> GetByIdAsync(int id)
        {
            var review = await _context.Reviews.Include(r => r.Customer)
                                                   .ThenInclude(c => c!.User)
                                               .Include(r => r.Room)
                                               .FirstOrDefaultAsync(r => r.ReviewId == id);
            if (review is null) return null;
            return MapToResponseDto(review);
        }
        // Create
        public async Task<ReviewResponseDto> CreateAsync(
            ReviewCreateDto dto)
        {
            await CheckCustomerId(dto.CustomerId);
            await CheckRoomId(dto.RoomId);
            await CheckBookingId(dto.BookingId);
            await CheckCustomerBookingRoom(dto.CustomerId, dto.RoomId, dto.BookingId);
            await CheckReviewExists(dto.CustomerId, dto.RoomId, dto.BookingId);
            var review = MapToEntity(dto);
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(review.ReviewId) 
                      ?? throw new NotFoundException("Không tìm thấy đánh giá vừa tạo!");
        }
        // Update
        public async Task<ReviewResponseDto?> UpdateAsync(int id, ReviewUpdateDto dto)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review is null) return null;
            if (review.Status != ReviewEnums.Pending)
                throw new BadRequestException("Chỉ có thể cập nhật đánh giá đang chờ xử lý!");
            UpdateEntity(review, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review is null) return false;
            if (review.Status != ReviewEnums.Pending)
                throw new BadRequestException("Chỉ có thể xóa đánh giá đang chờ xử lý!");
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
        // Xác nhận Review
        public async Task<bool> ConfirmAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review is null) return false;
            if (review.Status != ReviewEnums.Pending)
                throw new BadRequestException("Chỉ có thể xác nhận đánh giá đang chờ xử lý");
            review.Status = ReviewEnums.Approved;
            await _context.SaveChangesAsync();
            return true;
        }
        // Từ chối Review
        public async Task<bool> RejectAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review is null) return false;
            if (review.Status != ReviewEnums.Pending)
                throw new BadRequestException("Chỉ có thể xác nhận đánh giá đang chờ xử lý");
            review.Status = ReviewEnums.Approved;
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
