using HotelManagement.Data;
using HotelManagement.DTOs.Invoice;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class InvoiceService
    {
        private readonly AppDbContext _context;
        public InvoiceService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Invoice -> ResponseDto
        private async Task<InvoiceResponseDto> MapToResponseDto(Invoice invoice)
        {
            var paidAmount = await CalculatePaidAmount(invoice.InvoiceId);
            var remainingAmount = invoice.AmountDue - paidAmount;   // Số tiền còn lại phải trả
            if (remainingAmount < 0) remainingAmount = 0;   // Không thể âm
            return new InvoiceResponseDto
            {
                InvoiceId = invoice.InvoiceId,
                BookingId = invoice.BookingId,
                CustomerName = invoice.Booking?.Customer?.User?.FullName,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                DiscountAmount = invoice.DiscountAmount,
                AmountDue = invoice.AmountDue,
                PaidAmount = paidAmount,
                RemainingAmount = remainingAmount,
                Status = invoice.Status,
                CreatedByUserId = invoice.CreatedByUserId,
                CreateAt = invoice.CreateAt
            };
        }
        // Tính tổng tiền đã thanh toán thành công
        private async Task<decimal> CalculatePaidAmount(int invoiceId)
        {
            var payments = await _context.Payments
                                         .Where(p => p.InvoiceId == invoiceId && p.Status == PaymentEnums.Success)
                                         .ToListAsync();
            decimal paidAmount = 0;
            foreach (var payment in payments)
            {
                if (payment.PaymentType == PaymentTypeEnums.Refund) paidAmount -= payment.Amount;
                else paidAmount += payment.Amount;
            }
            if (paidAmount < 0) paidAmount = 0;
            return paidAmount;
        }
        // Kiểm tra BookingId
        private async Task<Booking> CheckBookingId(int bookingId)
        {
            var booking = await _context.Bookings
                                        .Include(b => b.Customer)
                                            .ThenInclude(c => c!.User)
                                        .FirstOrDefaultAsync(b => b.BookingId == bookingId
                                                               && b.IsDeleted == ActiveEnums.Active);
            if (booking is null)
                throw new NotFoundException("Mã đặt phòng không hợp lệ!");
            return booking;
        }
        // Kiểm tra Booking đã tạo Invoice chưa?
        private async Task CheckInvoiceBooking(int bookingId)
        {
            var invoiceExists = await _context.Invoices
                                              .AnyAsync(i => i.BookingId == bookingId);

            if (invoiceExists)
                throw new ConflictException("Đặt phòng này đã có hóa đơn!");
        }
        // Kiểm tra CreatedByUserId
        private async Task CheckCreateByUserId(int? userId)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .Select(u => new
                                           {
                                               u.UserId,
                                               u.Role!.RoleName
                                           })
                                           .FirstOrDefaultAsync(u => u.UserId == userId);
            if (user is null)
                throw new NotFoundException("Mã nhân viên tạo hóa đơn không hợp lệ!");
            if (user.RoleName != "Nhân viên")
                throw new BadRequestException("Mã người tạo này không phải là nhân viên!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all
        public async Task<List<InvoiceResponseDto>> GetAllAsync()
        {
            var invoices = await _context.Invoices
                                         .Include(i => i.Booking)
                                                .ThenInclude(b => b!.Customer)
                                                .ThenInclude(c => c!.User)
                                        .ToListAsync();
            var result = new List<InvoiceResponseDto>();
            foreach (var invoice in invoices)
            {
                result.Add(await MapToResponseDto(invoice));
            }
            return result;
        }
        // Get by Id
        public async Task<InvoiceResponseDto?> GetByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                                        .Include(i => i.Booking)
                                            .ThenInclude(b => b!.Customer)
                                            .ThenInclude(c => c!.User)
                                        .FirstOrDefaultAsync(i => i.InvoiceId == id);
            if (invoice is null) return null;
            return await MapToResponseDto(invoice);
        }
        // Create
        public async Task<InvoiceResponseDto> CreateAsync(InvoiceCreateDto dto)
        {
            var booking = await CheckBookingId(dto.BookingId);
            await CheckInvoiceBooking(dto.BookingId);
            await CheckCreateByUserId(dto.CreatedByUserId);
            var subTotal = booking.RoomAmount + booking.PackageAmount + booking.AdditionalServiceAmount;
            var discountAmount = booking.DiscountAmount;
            if (discountAmount > subTotal) 
                discountAmount = subTotal;
            var taxableAmount = subTotal - discountAmount;
            if (taxableAmount < 0)
                taxableAmount = 0;
            const decimal taxRate = 10m;
            var taxAmount = taxableAmount * taxRate / 100;
            var amountDue = taxableAmount + taxAmount;
            var invoice = new Invoice
            {
                BookingId = booking.BookingId,
                SubTotal = subTotal,
                TaxAmount = taxRate,
                DiscountAmount = discountAmount,
                AmountDue = amountDue,
                Status = InvoiceEnums.Unpaid,
                CreatedByUserId = dto.CreatedByUserId
            };
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(invoice.InvoiceId))!;
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var invoice = await _context.Invoices
                                        .Include(i => i.Payments)
                                        .FirstOrDefaultAsync(i => i.InvoiceId == id);
            if (invoice is null) return false;
            if (invoice.Payments.Any()) 
                throw new BadRequestException("Không thể xóa hóa đơn đã có giao dịch thanh toán!"); 
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
