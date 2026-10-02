using HotelManagement.Data;
using HotelManagement.DTOs.ApplyPromotion;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class ApplyPromotionService
    {
        private readonly AppDbContext _context;
        public ApplyPromotionService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển ApplyPromotion -> ApplyPromotionResponseDto
        private ApplyPromotionResponseDto MapToResponseDto(ApplyPromotion applyPromotion)
        {
            return new ApplyPromotionResponseDto
            {
                ApplyPromotionId = applyPromotion.ApplyPromotionId,
                // Thông tin Promotion
                PromotionId = applyPromotion.PromotionId,
                PromotionCode = applyPromotion.Promotion?.PromotionCode,
                PromotionName = applyPromotion.Promotion?.PromotionName,
                // Thông tin RoomType
                RoomTypeId = applyPromotion.RoomTypeId,
                RoomTypeName = applyPromotion.RoomType?.RoomTypeName,
                CreateAt = applyPromotion.CreateAt,
                UpdateAt = applyPromotion.UpdateAt
            };
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all 
        public async Task<List<ApplyPromotionResponseDto>> GetAllAsync()
        {
            var applies = await _context.ApplyPromotions.Select(a => new ApplyPromotionResponseDto
                                                            {
                                                                ApplyPromotionId = a.ApplyPromotionId,
                                                                PromotionId = a.PromotionId,
                                                                PromotionCode = a.Promotion!.PromotionCode,
                                                                PromotionName = a.Promotion!.PromotionName,
                                                                RoomTypeId = a.RoomTypeId,
                                                                RoomTypeName = a.RoomType!.RoomTypeName,
                                                                CreateAt = a.CreateAt,
                                                                UpdateAt = a.UpdateAt
                                                            }).ToListAsync();
            return applies;
        }
        // Get by Id
        public async Task<ApplyPromotionResponseDto?> GetByIdAsync(int id)
        {
            var apply = await _context.ApplyPromotions.Include(a => a.Promotion)
                                                      .Include(a => a.RoomType)
                                                      .FirstOrDefaultAsync(a => a.ApplyPromotionId == id);
            if (apply is null) return null;
            return MapToResponseDto(apply);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var apply = await _context.ApplyPromotions.FindAsync(id);
            if (apply is null) return false;
            _context.ApplyPromotions.Remove(apply);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
