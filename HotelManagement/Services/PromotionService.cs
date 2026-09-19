using HotelManagement.Data;
using HotelManagement.DTOs.Promotion;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class PromotionService
    {
        private readonly AppDbContext _context;
        public PromotionService(AppDbContext context)
        {
            _context = context;
        }
        private PromotionResponseDto MapToResponseDto(Promotion promotion)
        {
            return new PromotionResponseDto
            {
                PromotionId = promotion.PromotionId,
                PromotionCode= promotion.PromotionCode,
                PromotionName= promotion.PromotionName,
                DiscountValue= promotion.DiscountValue,
                MinAmount= promotion.MinAmount,
                MaxDisCountAmount = promotion.MaxDisCountAmount,
                UsageLimit= promotion.UsageLimit,
                UsedCount = promotion.UsedCount,
                StartDate= promotion.StartDate,
                EndDate = promotion.EndDate,
                IsDeleted = promotion.IsDeleted,
                CreateAt = promotion.CreateAt,
                UpdateAt = promotion.UpdateAt
            };
        }
        // Chuyển CreateDto -> Promotion
        private Promotion MapToEntity(PromotionCreateDto dto)
        {
            return new Promotion
            {
                PromotionCode = dto.PromotionCode,
                PromotionName = dto.PromotionName,
                DiscountValue = dto.DiscountValue,
                MinAmount = dto.MinAmount,
                MaxDisCountAmount = dto.MaxDisCountAmount,
                UsageLimit = dto.UsageLimit,
                UsedCount = dto.UsedCount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
        }
        // Update Promotion từ PromotionUpdateDto
        private void UpdateEntity(Promotion promotion, PromotionUpdateDto dto)
        {
            promotion.PromotionCode = dto.PromotionCode;
            promotion.PromotionName = dto.PromotionName;
            promotion.DiscountValue = dto.DiscountValue;
            promotion.MinAmount = dto.MinAmount;
            promotion.MaxDisCountAmount = dto.MaxDisCountAmount;
            promotion.UsageLimit = dto.UsageLimit;
            promotion.UsedCount = dto.UsedCount;
            promotion.StartDate = dto.StartDate;
            promotion.EndDate = dto.EndDate;
        }
        // Check trùng PromotionCode
        private async Task<bool> PromotionCodeExists(string promotionCode, int? promotionId = null)
        {
            return await _context.Promotions.AnyAsync(p => p.PromotionCode == promotionCode
                                                  && (promotionId == null || promotionId != p.PromotionId));
        }
        // Check trùng PromotionName
        private async Task<bool> PromotionNameExists(string promotionName, int? promotionId = null)
        {
            return await _context.Promotions.AnyAsync(p => p.PromotionName == promotionName
                                                    && (promotionId == null || promotionId != p.PromotionId));
        }
        // Check Date
        private void CheckDate(DateTime startDate, DateTime endDate)
        {
            if (startDate < DateTime.Today)
                throw new BadRequestException("Ngày bắt đầu áp dụng không được trước hôm nay!");
            if (endDate < startDate)
                throw new BadRequestException("Ngày kết thúc áp dụng không được trước ngày bắt đầu áp dụng!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all Active(Lấy promotion còn hoạt động)
        public async Task<List<PromotionResponseDto>> GetAllAsync()
        {
            var promotions = await _context.Promotions.Where(p => p.IsDeleted == ActiveEnums.Active)
                                            .Select(p => new PromotionResponseDto
                                            {
                                                PromotionId = p.PromotionId,
                                                PromotionCode = p.PromotionCode,
                                                PromotionName = p.PromotionName,
                                                DiscountValue = p.DiscountValue,
                                                MinAmount = p.MinAmount,
                                                MaxDisCountAmount = p.MaxDisCountAmount,
                                                UsageLimit = p.UsageLimit,
                                                UsedCount = p.UsedCount,
                                                StartDate = p.StartDate,
                                                EndDate = p.EndDate,
                                                IsDeleted = p.IsDeleted,
                                                CreateAt = p.CreateAt,
                                                UpdateAt = p.UpdateAt
                                            }).ToListAsync();
            return promotions;
        }
        // Get all InActive(Lấy promotion không hoạt động)
        public async Task<List<PromotionResponseDto>> GetDeletedAsync()
        {
            var promotions = await _context.Promotions.Where(p => p.IsDeleted == ActiveEnums.Inactive)
                                            .Select(p => new PromotionResponseDto
                                            {
                                                PromotionId = p.PromotionId,
                                                PromotionCode = p.PromotionCode,
                                                PromotionName = p.PromotionName,
                                                DiscountValue = p.DiscountValue,
                                                MinAmount = p.MinAmount,
                                                MaxDisCountAmount = p.MaxDisCountAmount,
                                                UsageLimit = p.UsageLimit,
                                                UsedCount = p.UsedCount,
                                                StartDate = p.StartDate,
                                                EndDate = p.EndDate,
                                                IsDeleted = p.IsDeleted,
                                                CreateAt = p.CreateAt,
                                                UpdateAt = p.UpdateAt
                                            }).ToListAsync();
            return promotions;
        }
        // Get by Id
        public async Task<PromotionResponseDto?> GetByIdAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion is null) return null;
            return MapToResponseDto(promotion);
        }
        // Create 
        public async Task<PromotionResponseDto> CreateAsync(PromotionCreateDto dto)
        {
            if (await PromotionCodeExists(dto.PromotionCode!))
                throw new ConflictException("Mã khuyến mãi đã tồn tại!");
            if (await PromotionNameExists(dto.PromotionName!))
                throw new ConflictException("Tên khuyến mãi đã tồn tại!");
            CheckDate(dto.StartDate, dto.EndDate);
            var promotion = MapToEntity(dto);
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
            return MapToResponseDto(promotion);
        }
        // Update
        public async Task<PromotionResponseDto?> UpdateAsync(int id, PromotionUpdateDto dto)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion is null) return null;
            if (await PromotionCodeExists(dto.PromotionCode!, id))
                throw new ConflictException("Mã khuyến mãi đã tồn tại!");
            if (await PromotionNameExists(dto.PromotionName!, id))
                throw new ConflictException("Tên khuyến mãi đã tồn tại!");
            CheckDate(dto.StartDate, dto.EndDate);
            UpdateEntity(promotion, dto);
            await _context.SaveChangesAsync();
            return MapToResponseDto(promotion);
        }
        // Delete
        public async Task<PromotionResponseDto?> DeleteAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion is null) return null;
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return MapToResponseDto(promotion);
        }
        // Xóa tạm
        public async Task<PromotionResponseDto?> SoftDeleteAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion is null) return null;
            promotion.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return MapToResponseDto(promotion);
        }
        // Khôi phục (Restore)
        public async Task<PromotionResponseDto?> RestoreAsync(int id)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(s => s.PromotionId == id);
            if (promotion is null) return null;
            promotion.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return MapToResponseDto(promotion);
        }
    }
}
