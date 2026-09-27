using HotelManagement.Data;
using HotelManagement.DTOs.Payment;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;
        public PaymentService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Payment -> ResponseDto
        private PaymentResponseDto MapToResponseDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                PaymentType = payment.PaymentType,
                PaymentMethod = payment.PaymentMethod,
                TransactionCode = payment.TransactionCode,
                Status = payment.Status,
                CreatedByUserId = payment.CreatedByUserId,
                PaymentDate = payment.PaymentDate,
                CreateAt = payment.CreateAt
            };
        }
        //  Chuyển CreateDto -> Payment
        private Payment MapToEntity(PaymentCreateDto dto)
        {
            return new Payment
            {
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                PaymentType = dto.PaymentType,
                PaymentMethod = dto.PaymentMethod,
                TransactionCode = dto.TransactionCode,
                CreatedByUserId = dto.CreatedByUserId,
                PaymentDate = dto.PaymentDate,
            };
        }
        // Check InvoiceId có tồn tại trong bảng Invoice không?
        private async Task<Invoice> CheckInvoiceId(int invoiceId)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice is null)
                throw new NotFoundException("Mã hóa đơn không hợp lệ!");
            return invoice;
        }
        // Check CreatedByUserId có tồn tại trong bảng User không?
        // Check hợp lệ: Vai trò phải là nhân viên
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
                throw new NotFoundException("Mã nhân viên tạo thanh toán không hợp lệ!");
            if (user.RoleName != "Nhân viên")
                throw new BadRequestException("Mã người tạo này không phải là nhân viên!");
        }
        // Tính số tiền đã thanh toán
        private async Task<decimal> CalculatePaidAmount(int invoiceId)
        {
            var payments = await _context.Payments
                                        .Where(p => p.InvoiceId == invoiceId
                                                 && p.Status == PaymentEnums.Success)
                                        .Select(p => new
                                        {
                                            p.Amount,
                                            p.PaymentType
                                        })
                                       .ToListAsync();
            decimal paidAmount = 0;
            foreach (var payment in payments)
            {
                if (payment.PaymentType == PaymentTypeEnums.Refund)
                    paidAmount -= payment.Amount;
                else
                    paidAmount += payment.Amount;
            }
            // Số tiền thanh toán không được âm
            if (paidAmount < 0) paidAmount = 0;
            return paidAmount;
        } 
        // Check số tiền thanh toán có vượt quá số tiền phải trả trong hóa đơn không?
        private async Task CheckAmount(int invoiceId, decimal amount, PaymentTypeEnums paymentType)
        {
            var invoice = await _context.Invoices
                                        .Select(i => new 
                                        { 
                                            i.InvoiceId, 
                                            i.AmountDue 
                                        })
                                        .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice is null) 
                throw new NotFoundException("Mã hóa đơn không hợp lệ!");
            // Tính paymnet
            var paidAmount = await CalculatePaidAmount(invoiceId);
            // Refund
            if (paymentType == PaymentTypeEnums.Refund)
            {
                if (amount > paidAmount)
                    throw new BadRequestException("Số tiền hoàn trả không được vượt quá số tiền đã thanh toán!");
                return;
            }
            // Payment / Deposit
            if (paymentType == PaymentTypeEnums.Payment || paymentType == PaymentTypeEnums.Deposit)
            {
                var remainingAmount = invoice.AmountDue - paidAmount;
                if (remainingAmount < 0) 
                    remainingAmount = 0;
                if (amount > remainingAmount)
                    throw new BadRequestException("Số tiền thanh toán đã vượt quá số tiền còn phải trả!");
            }    
        }
        // Cập nhật trạng thái của Invoice
        private async Task UpdateInvoiceStatus(int invoiceId)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            if (invoice is null)
                throw new NotFoundException("Mã hóa đơn không hợp lệ!");
            // Lấy toàn bdanh sách thanh toán của InvoiceId
            var payments = await _context.Payments.Where(p => p.InvoiceId == invoiceId
                                                           && p.Status == PaymentEnums.Success)
                                           .Select(p => new
                                           {
                                               p.Amount,
                                               p.PaymentType
                                           }).ToListAsync();
            decimal paidAmount = 0;
            foreach (var payment in payments)
            {
                if (payment.PaymentType == PaymentTypeEnums.Refund)
                    paidAmount -= payment.Amount;
                else
                    paidAmount += payment.Amount;
            }
            if (paidAmount <= 0)
                invoice.Status = InvoiceEnums.Unpaid;
            else if (paidAmount < invoice.AmountDue)
                invoice.Status = InvoiceEnums.PartiallyPaid;
            else
                invoice.Status = InvoiceEnums.Paid;
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all
        public async Task<List<PaymentResponseDto>> GetAllAsync()
        {
            var payments = await _context.Payments
                                         .Select(p => new PaymentResponseDto
                                         {
                                             PaymentId = p.PaymentId,
                                             InvoiceId = p.InvoiceId,
                                             Amount = p.Amount,
                                             PaymentType = p.PaymentType,
                                             PaymentMethod = p.PaymentMethod,
                                             TransactionCode = p.TransactionCode,
                                             Status = p.Status,
                                             CreatedByUserId = p.CreatedByUserId,
                                             PaymentDate = p.PaymentDate,
                                             CreateAt = p.CreateAt
                                         })
                                        .ToListAsync();
            return payments;
        }
        // Get by Id
        public async Task<PaymentResponseDto?> GetByIdAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return null;
            return MapToResponseDto(payment);
        }
        // Create
        public async Task<PaymentResponseDto> CreateAsync(PaymentCreateDto dto)
        {
            await CheckInvoiceId(dto.InvoiceId);
            await CheckAmount(dto.InvoiceId, dto.Amount, dto.PaymentType);
            await CheckCreateByUserId(dto.CreatedByUserId);
            var payment = MapToEntity(dto);
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return MapToResponseDto(payment);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return false;
            // Không cho xóa giao dịch đã thành công
            if (payment.Status == PaymentEnums.Success)
                throw new BadRequestException("Không thể xóa giao dịch thanh toán đã thành công!");
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }
        // Xác nhận thanh toán
        public async Task<bool?> ConfirmAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return false;
            // Chỉ được xác nhận thanh toán đang Pending
            if (payment.Status != PaymentEnums.Pending)
                throw new BadRequestException("Chỉ được xác nhận giao dịch đang chờ xử lý!");
            // Kiểm tra lại số tiền tại thời điểm xác nhận
            // Do có thể Invoice đã có 1 thanh toán mới trước đó
            await CheckAmount(payment.InvoiceId, payment.Amount, payment.PaymentType);
            payment.Status = PaymentEnums.Success;
            payment.PaymentDate = DateTime.Now;
            await UpdateInvoiceStatus(payment.InvoiceId);
            await _context.SaveChangesAsync();
            return true;
        }
        // Từ chối thanh toán
        public async Task<bool?> RejectAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return false;
            // Chỉ được xác nhận thanh toán đang Pending
            if (payment.Status != PaymentEnums.Pending)
                throw new BadRequestException("Chỉ được từ chối giao dịch đang chờ xử lý!");
            payment.Status = PaymentEnums.Failed;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
