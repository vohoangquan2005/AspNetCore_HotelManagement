using HotelManagement.Data;
using HotelManagement.DTOs.Booking;
using HotelManagement.DTOs.BookingDetail;
using HotelManagement.Enums;
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
        // ===========================================================
        // =                        MAPPING                          =
        // ===========================================================
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
                IsDeleted = booking.IsDeleted,
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
                ExpectedCheckIn = dto.ExpectedCheckIn,
                ExpectedCheckOut = dto.ExpectedCheckOut,
                GuestCount = dto.GuestCount,
                RoomCount = dto.RoomCount,
                PromotionId = dto.PromotionId,
                ServicePackageId = dto.ServicePackageId,
                Note = dto.Note,
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
            booking.Note = dto.Note;
        }
        // ===========================================================
        // =                       VALIDATION                        =
        // ===========================================================
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
            var exists = await _context.Promotions.AnyAsync(p => p.PromotionId == promotionId
                                                              && p.IsDeleted == ActiveEnums.Active);
            if (!exists)
                throw new NotFoundException("Mã khuyến mãi không tồn tại!");
        }
        // Check ServicePackageId có tồn tại trong bảng ServicePackage không?
        private async Task CheckServicePackageId(int? servicePackageId)
        {
            if (servicePackageId is null) return;
            var exists = await _context.ServicePackages.AnyAsync(sp => sp.ServicePackageId == servicePackageId
                                                                    &&(sp.IsDeleted == ActiveEnums.Active));
            if (!exists)
                throw new NotFoundException("Gói dịch vụ không tồn tại!");
        }
        // Check Date
        private void CheckDate(DateTime expectedCheckIn, DateTime expectedCheckOut)
        {
            if (expectedCheckIn >= expectedCheckOut)
                throw new BadRequestException("Ngày trả phòng phải sau ngày nhận phòng!");
            if (expectedCheckIn < DateTime.Now)
                throw new BadRequestException("Ngày nhận phòng không được ở trong quá khứ!");
        }
        // ===========================================================
        // =                  CHECK BOOKING DETAILS                  =
        // ===========================================================
        // Check danh sách phòng trong BookingDetail
        private async Task CheckBookingDetails(List<BookingDetailCreateDto> bookingDetails, int roomCount,int guestCount)
        {
            // Phải có ít nhất một phòng
            if (bookingDetails is null || !bookingDetails.Any())
                throw new BadRequestException("Booking phải có ít nhất một phòng!");
            // Số lượng phòng phải khớp với số BookingDetail
            if (bookingDetails.Count != roomCount)
                throw new BadRequestException(
                    "Số lượng phòng không khớp với danh sách phòng!");
            // Không cho phép cùng một phòng xuất hiện nhiều lần
            var duplicateRoom = bookingDetails.GroupBy(bd => bd.RoomId).Any(g => g.Count() > 1);
            if (duplicateRoom)
                throw new ConflictException("Không được chọn cùng một phòng nhiều lần!");
            // Tổng số khách trong các phòng
            var totalGuestCount = bookingDetails.Sum(bd => bd.GuestCount);
            if (totalGuestCount != guestCount)
                throw new BadRequestException("Tổng số khách trong các phòng không khớp với số lượng khách của Booking!");
            // Kiểm tra từng phòng
            foreach (var detail in bookingDetails)
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == detail.RoomId);
                if (room is null)
                    throw new NotFoundException($"Phòng {detail.RoomId} không tồn tại!");
                if (detail.GuestCount > room.Capacity) 
                    throw new BadRequestException($"Phòng {detail.RoomId} chỉ có sức chứa {room.Capacity} khách!"); 
            }
        }
        // ===========================================================
        // =                    CHECK ROOM AVAILABLE                 =
        // ===========================================================
        // Check phòng có được yêu cầu bởi 1 Booking khác trong khoảng thời gian ExpectedCheckIn -> ExpectedCheckOut hay không?
        private async Task CheckRoomAvailable(int roomId, DateTime expectedCheckIn, DateTime expectedCheckOut, int? bookingId = null)
        {
            var query = _context.BookingDetails
                                              .Where(bd => bd.RoomId == roomId
                                                        && bd.Booking!.IsDeleted == ActiveEnums.Active
                                                        && bd.Booking!.Status != BookingEnums.Cancelled
                                                        && bd.Booking.ExpectedCheckIn < expectedCheckOut
                                                        && bd.Booking.ExpectedCheckOut > expectedCheckIn);
            // Nếu Update thì bỏ qua Booking hiện tại
            if (bookingId.HasValue)
                query = query.Where(bd => bd.BookingId != bookingId.Value);
            var bookingExists = await query.AnyAsync();
            if (bookingExists)
                throw new ConflictException($"Phòng {roomId} đã được đặt trong khoảng " + $"{expectedCheckIn:dd/MM/yyyy HH:mm} đến " + $"{expectedCheckOut:dd/MM/yyyy HH:mm}!");                                 
        }
        // ===========================================================
        // =                 CHECK VALID PROMOTION                   =
        // ===========================================================
        // Check Promotion hợp lệ hay không?
        private async Task<Promotion?> CheckValidPromotion(int? promotionId)
        {
            if (promotionId == null) return null;
            var now = DateTime.Now;
            var promotion = await _context.Promotions
                                          .FirstOrDefaultAsync(p => p.PromotionId == promotionId
                                                                 && p.IsDeleted == ActiveEnums.Active
                                                                 && p.StartDate <= now
                                                                 && p.EndDate >= now);
            if (promotion is null)
                throw new BadRequestException("Mã khuyến mãi không tồn tại hoặc đã hết hạn!");
            // Kiểm tra giới hạn sử dụng
            if (promotion.UsageLimit.HasValue && (promotion.UsedCount ?? 0) >= promotion.UsageLimit.Value)
                throw new BadRequestException("Mã khuyến mãi đã hết lượt sử dụng!");
            return promotion;
        }
        private void IncreasePromotionUsedCount( Promotion promotion)
        { 
            promotion.UsedCount = (promotion.UsedCount ?? 0) + 1; 
        }
        // ===========================================================
        // =                CREATE BOOKING DETAILS                   =
        // ===========================================================
        // Tạo chi tiết đặt phòng (BookingDetail)
        private async Task CreateBookingDetails(int bookingId, List<BookingDetailCreateDto> details)
        {
            foreach (var detail in details) 
            {
                var room = await _context.Rooms.FindAsync(detail.RoomId);
                if (room is null)
                    throw new NotFoundException($"Phòng có mã {detail.RoomId} không tồn tại!");
                var bookingDetail = new BookingDetail
                {
                    BookingId = bookingId,
                    RoomId = detail.RoomId,
                    // Lưu giá tại thời điểm đặt phòng
                    PricePerNight = room.PricePerNight,
                    GuestCount = detail.GuestCount,
                    ActualCheckIn = null,
                    ActualCheckOut = null
                };
                await _context.BookingDetails.AddAsync(bookingDetail);
            } 
        }
        // ===========================================================
        // =                    CALCULATE MONEY                      =
        // ===========================================================
        // Tính RoomAmount
        private decimal CalculateRoomAmount(IEnumerable<BookingDetail> bookingDetails, DateTime expectedCheckIn, DateTime expectedCheckOut)
        {
            var nightNumber =(expectedCheckOut.Date - expectedCheckIn.Date).Days;
            if (nightNumber <= 0)
                throw new BadRequestException("Số đêm lưu trú phải lớn hơn 0!");

            return bookingDetails.Sum(detail => detail.PricePerNight * nightNumber);
        }
        // Tính PackageAmount
        private async Task<decimal> CalculatePackageAmount(int? servicePackageId)
        {
            if (servicePackageId is null) return 0;
            var package = await _context.ServicePackages.FindAsync(servicePackageId);
            if (package is null)
                throw new NotFoundException("Gói dịch vụ không tồn tại!");
            return package.PackagePrice;
        }
        // Tính DiscountAmount
        private decimal CalculateDisCountAmount(Promotion? promotion, decimal roomAmount, decimal packageAmount)
        {
            if (promotion is null || promotion.DiscountValue is null) return 0;
            var amountBeforeDiscount = roomAmount + packageAmount;
            // Check giá trị tối thiểu được giảm
            if (promotion.MinAmount.HasValue && amountBeforeDiscount < promotion.MinAmount.Value)
                return 0;
            // Tính tiền giảm
            var discountValue = promotion.DiscountValue ?? 0;
            var amountAfterDiscount = amountBeforeDiscount * discountValue / 100;
            // Check tiền giảm vượt giới hạn giảm không?
            if (promotion.MaxDisCountAmount.HasValue && amountAfterDiscount > promotion.MaxDisCountAmount.Value)
                amountAfterDiscount = promotion.MaxDisCountAmount.Value;
            return amountAfterDiscount;
        }
        // Tính TotalAmount
        private decimal CalculateTotalAmount(decimal roomAmount, decimal packageAmount, decimal additionalServiceAmount, decimal discountAmount)
        {
            var totalAmount = roomAmount + packageAmount + additionalServiceAmount - discountAmount;
            if(totalAmount < 0) return 0;
            return totalAmount;
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all (Lấy danh sách booking đang hoạt động)
        public async Task<List<BookingResponseDto>> GetAllAsync()
        {
            var bookings = await _context.Bookings.Where(b => b.IsDeleted == ActiveEnums.Active)
                                        .OrderByDescending(b => b.BookingDate)
                                        .Select(b => new BookingResponseDto
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
                                            IsDeleted = b.IsDeleted,
                                            CreateAt = b.CreateAt,
                                            UpdateAt = b.UpdateAt
                                        }).ToListAsync();
            return bookings;
        }
        // Get deleted (Lấy danh sách booking không hoạt động)
        public async Task<List<BookingResponseDto>> GetDeletedAsync()
        {
            var bookings = await _context.Bookings.Where(b => b.IsDeleted == ActiveEnums.Inactive)
                                        .OrderByDescending(b => b.BookingDate)
                                        .Select(b => new BookingResponseDto
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
                                            IsDeleted = b.IsDeleted,
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
                                                 .FirstOrDefaultAsync(b => b.BookingId == id
                                                                        && b.IsDeleted == ActiveEnums.Active);
            if (booking is null) return null;
            return MapToResponseDto(booking);
        }
        // Create 
        public async Task<BookingResponseDto> CreateAsync(BookingCreateDto dto)
        {
            // Dùng Transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check đầu vào
                await CheckCustomerId(dto.CustomerId);
                await CheckPromotionId(dto.PromotionId);
                var promotion = await CheckValidPromotion(dto.PromotionId);
                await CheckServicePackageId(dto.ServicePackageId);
                CheckDate(dto.ExpectedCheckIn, dto.ExpectedCheckOut);
                await CheckBookingDetails(dto.BookingDetails, dto.RoomCount, dto.GuestCount);
                // Check tất cả phòng còn trống không?
                foreach (var detail in dto.BookingDetails)
                {
                    await CheckRoomAvailable(detail.RoomId, dto.ExpectedCheckIn, dto.ExpectedCheckOut);
                }
                // Tạo booking
                var booking = MapToEntity(dto);
                // Các khoản tiền ban đầu
                booking.DepositAmount = 0;
                booking.RoomAmount = 0;
                booking.PackageAmount = 0;
                booking.DiscountAmount = 0;
                booking.AdditionalServiceAmount = 0;
                booking.TotalAmount = 0;
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                // Tạo booking detail và tính tiền phòng
                await CreateBookingDetails(booking.BookingId, dto.BookingDetails);
                await _context.SaveChangesAsync();
                var listBookingDetails = await _context.BookingDetails
                                                       .Where(bd => bd.BookingId == booking.BookingId)
                                                       .ToListAsync();
                var roomAmount = CalculateRoomAmount(listBookingDetails, dto.ExpectedCheckIn, dto.ExpectedCheckOut);
                // Tính tiền ServicePackage
                var packageAmount = await CalculatePackageAmount(dto.ServicePackageId);
                // Tính tiền giảm
                var discountAmount = CalculateDisCountAmount(promotion, roomAmount, packageAmount);
                // Tiền dịch vụ đặt thêm ban đầu = 0
                var additionalServiceAmount = 0m;
                // Tính tổng tiền
                var totalAmount = CalculateTotalAmount(roomAmount, packageAmount, additionalServiceAmount, discountAmount);
                // Gán các khoản tiền vào booking
                booking.RoomAmount = roomAmount;
                booking.PackageAmount = packageAmount;
                booking.DiscountAmount = discountAmount;
                booking.AdditionalServiceAmount = additionalServiceAmount;
                booking.TotalAmount = totalAmount;
                // Tăng UsedCount
                if (promotion is not null)
                    IncreasePromotionUsedCount(promotion);
                await _context.SaveChangesAsync();
                // Commit
                await transaction.CommitAsync();
                return (await GetByIdAsync(booking.BookingId))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Update
        public async Task<BookingResponseDto?> UpdateAsync(int id, BookingUpdateDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                                            .Include(b => b.BookingDetails)
                                            .FirstOrDefaultAsync(b => b.BookingId == id
                                                                   && b.IsDeleted == ActiveEnums.Active);
                if (booking is null) return null;
                // Chỉ cho phép sửa các Booking đang Pending
                if (booking.Status != BookingEnums.Pending) 
                    throw new BadRequestException("Chỉ có thể cập nhật Booking đang ở trạng thái Pending!"); 
                // Không cho update Booking đã hủy đặt phòng
                if (booking.Status == BookingEnums.Cancelled)
                    throw new BadRequestException("Không thể cập nhật Booking đã hủy!");
                // Lưu thông tin khuyến mãi và gói dịch vụ cũ
                var oldPromotionId = booking.PromotionId;
                // Validate
                await CheckCustomerId(dto.CustomerId);
                await CheckPromotionId(dto.PromotionId);
                var newPromotion = await CheckValidPromotion(dto.PromotionId);
                await CheckServicePackageId(dto.ServicePackageId);
                CheckDate(dto.ExpectedCheckIn, dto.ExpectedCheckOut);
                // Kiểm tra số lượng phòng
                var currentDetailCount = booking.BookingDetails.Count;
                if (dto.RoomCount != currentDetailCount) 
                    throw new BadRequestException("Không thể thay đổi số lượng phòng khi cập nhật Booking. " + "Hãy cập nhật BookingDetail tương ứng!");
                // Kiểm tra tổng số khách
                var currentGuestCount = booking.BookingDetails.Sum(bd => bd.GuestCount);
                if (dto.GuestCount != currentGuestCount)
                    throw new BadRequestException("Số lượng khách không khớp với chi tiết đặt phòng!");
                // Check cho tất cả phòng còn trống
                foreach (var detail in booking.BookingDetails)
                {
                    await CheckRoomAvailable(detail.RoomId, dto.ExpectedCheckIn, dto.ExpectedCheckOut, booking.BookingId);
                }
                // Tính lại các khoản tiền
                // Tính RoomAmount
                var listBookingDetails = await _context.BookingDetails
                                                       .Where(bd => bd.BookingId == booking.BookingId)
                                                       .ToListAsync();
                var roomAmount = CalculateRoomAmount(listBookingDetails, dto.ExpectedCheckIn, dto.ExpectedCheckOut);
                // Tính tiền ServicePackage
                var packageAmount = await CalculatePackageAmount(dto.ServicePackageId);
                // Tính tiền giảm
                var discountAmount = CalculateDisCountAmount(newPromotion, roomAmount, packageAmount);
                // Tiền dịch vụ đặt thêm ban đầu = 0
                var additionalServiceAmount = booking.AdditionalServiceAmount;
                // Tính tổng tiền
                var totalAmount = CalculateTotalAmount(roomAmount, packageAmount, additionalServiceAmount, discountAmount);
                // Gán lại các khoản tiền
                booking.RoomAmount = roomAmount;
                booking.PackageAmount = packageAmount;
                booking.DiscountAmount = discountAmount;
                booking.AdditionalServiceAmount = additionalServiceAmount;
                booking.TotalAmount = totalAmount;
                // Xử lý UsedCount của Promotion
                if (oldPromotionId != dto.PromotionId)
                {
                    if (oldPromotionId.HasValue)
                    {
                        var oldPromotion = await _context.Promotions
                                                         .FirstOrDefaultAsync(p => p.PromotionId == oldPromotionId.Value);
                        if (oldPromotion is not null && oldPromotion.UsedCount.HasValue)
                        {
                            oldPromotion.UsedCount--;
                            if (oldPromotion.UsedCount < 0)
                                oldPromotion.UsedCount = 0;
                        }
                    }
                    if (newPromotion is not null)
                        IncreasePromotionUsedCount(newPromotion);
                }
                UpdateEntity(booking, dto);
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
        // Soft delete
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var booking = await _context.Bookings
                                        .FirstOrDefaultAsync(b => b.BookingId == id
                                                               && b.IsDeleted == ActiveEnums.Active);

            if (booking is null) return false;
            booking.IsDeleted = ActiveEnums.Inactive;
            booking.UpdateAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Chỉ cho phép delete Booking đã soft delete
                var booking = await _context.Bookings
                                            .FirstOrDefaultAsync(b => b.BookingId == id
                                                                   && b.IsDeleted == ActiveEnums.Inactive);
                if (booking is null) return false;
                // Không cho xóa nếu Booking đã có Invoice
                var hasInvoice = await _context.Invoices.AnyAsync(i => i.BookingId == id);
                if (hasInvoice)
                    throw new BadRequestException("Không thể xóa Booking vì Booking đã phát sinh hóa đơn!");
                // Không cho xóa nếu Booking đã có Cancellation
                var hasCancellation = await _context.Cancellations.AnyAsync(c => c.BookingId == id);
                if (hasCancellation)
                    throw new BadRequestException("Không thể xóa Booking vì Booking đã có lịch sử hủy!");
                // Không cho xóa nếu Booking đã có Review
                var hasReview = await _context.Reviews.AnyAsync(r => r.BookingId == id);
                if (hasReview)
                    throw new BadRequestException("Không thể xóa Booking vì Booking đã có đánh giá!");
                // BookingDetails và Guests sẽ được xóa thông qua Cascade Delete
                // BookingDetails -> Guests
                // BookingServices cũng sẽ được xóa thông qua Cascade Delete
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Auto delete (Xóa tất cả những booking không hoạt động
        public async Task<int> AutoSoftDeleteExpiredAsync()
        {
            var now = DateTime.Now;
            var bookings = await _context.Bookings
                                         .Where(b => b.IsDeleted == ActiveEnums.Active
                                                     && b.ExpectedCheckOut < now
                                                     && b.Status != BookingEnums.Cancelled)
                                         .ToListAsync();
            if (!bookings.Any()) return 0;
            foreach (var booking in bookings)
            {
                booking.Status = BookingEnums.Completed;
                booking.IsDeleted = ActiveEnums.Inactive;
            }
            await _context.SaveChangesAsync();
            return bookings.Count;
        }
        // Restore
        public async Task<bool> RestoreAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                                            .Include(b => b.BookingDetails)
                                            .FirstOrDefaultAsync(b => b.BookingId == id
                                                                   && b.IsDeleted == ActiveEnums.Inactive);
                if (booking is null) return false;
                // Không khôi phục Booking đã bị hủy
                if (booking.Status == BookingEnums.Cancelled)
                    throw new BadRequestException("Không thể khôi phục Booking đã bị hủy!");
                // Kiểm tra tất cả phòng còn trống
                foreach (var detail in booking.BookingDetails)
                {
                    await CheckRoomAvailable(
                        detail.RoomId,
                        booking.ExpectedCheckIn,
                        booking.ExpectedCheckOut,
                        booking.BookingId);
                }
                // Khôi phục Booking
                booking.IsDeleted = ActiveEnums.Active;
                booking.UpdateAt = DateTime.Now;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Xác nhận đặt phòng
        public async Task<BookingResponseDto?> ConfirmAsync(int id)
        {
            var booking = await _context.Bookings
                                        .FirstOrDefaultAsync(b => b.BookingId == id
                                                               && b.IsDeleted == ActiveEnums.Active);
            if (booking is null) return null;
            if (booking.Status != BookingEnums.Pending)
                throw new BadRequestException("Chỉ có thể xác nhận đặt phòng đang chờ xử lý!");
            booking.Status = BookingEnums.Confirmed;
            booking.UpdateAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
    }
}
