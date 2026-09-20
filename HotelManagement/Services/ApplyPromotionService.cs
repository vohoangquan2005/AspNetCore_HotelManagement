using HotelManagement.Data;
using HotelManagement.DTOs.ApplyPromotion;
using HotelManagement.Exceptions;
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
        // Chuyển ApplyPromotionCreateDto -> ApplyPromotion
        private ApplyPromotion MapToEntity(ApplyPromotionCreateDto dto)
        {
            return new ApplyPromotion
            {
                PromotionId = dto.PromotionId,
                RoomTypeId = dto.RoomTypeId
            };
        }
        // Cập nhật Promotion từ ApplyPromotionUpdateDto
        private void UpdateEntity(ApplyPromotion applyPromotion, ApplyPromotionUpdateDto dto)
        {
            applyPromotion.PromotionId = dto.PromotionId;
            applyPromotion.RoomTypeId = dto.RoomTypeId;
        }
        // Check PromotionId có tồn tại trong bảng Promotion không?
        private async Task CheckPromotionId(int promotionId)
        {
            var exists = await _context.Promotions.AnyAsync(p => p.PromotionId == promotionId);
            if (!exists)
                throw new NotFoundException("Mã khuyến mãi không tồn tại!");
        }
        // Check RoomTypeId có tồn tại trong bảng RoomType không?
        private async Task CheckRoomTypeId(int roomTypeId)
        {
            var exists = await _context.RoomTypes.AnyAsync(a => a.RoomTypeId == roomTypeId);
            if (!exists)
                throw new NotFoundException("Loại phòng không tồn tại!");
        }
        // Check khuyến mãi PromotionId đã áp dụng cho loại phòng RoomTypeId chưa?
        private async Task CheckApply(int promotionId, int roomTypeId, int? applyPromotionId = null)
        {
            var exists = await _context.ApplyPromotions.AnyAsync(a => a.PromotionId == promotionId && a.RoomTypeId == roomTypeId
                                                             && (applyPromotionId == null || applyPromotionId != a.ApplyPromotionId));
            if (exists)
                throw new ConflictException("Khuyến mãi đã được áp dụng cho loại phòng này. Không thể áp dụng tiếp!");
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
        // Create 
        public async Task<ApplyPromotionResponseDto> CreateAsync(ApplyPromotionCreateDto dto)
        {
            await CheckPromotionId(dto.PromotionId);
            await CheckRoomTypeId(dto.RoomTypeId);
            await CheckApply(dto.PromotionId, dto.RoomTypeId);
            var apply = MapToEntity(dto);
            await _context.ApplyPromotions.AddAsync(apply);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(apply.ApplyPromotionId))!;
        }
        // Update
        public async Task<ApplyPromotionResponseDto?> UpdateAsync(int id, ApplyPromotionUpdateDto dto)
        {
            var apply = await _context.ApplyPromotions.FindAsync(id);
            if (apply is null) return null;
            await CheckPromotionId(dto.PromotionId);
            await CheckRoomTypeId(dto.RoomTypeId);
            await CheckApply(dto.PromotionId, dto.RoomTypeId, id);
            UpdateEntity(apply, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
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
