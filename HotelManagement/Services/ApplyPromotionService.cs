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
        private async Task<bool> CheckPromotionId(int promotionId)
        {
            return await _context.Promotions.AnyAsync(p => p.PromotionId == promotionId);
        }
        // Check RoomTypeId có tồn tại trong bảng RoomType không?
        private async Task<bool> CheckRoomTypeId(int roomTypeId)
        {
            return await _context.RoomTypes.AnyAsync(a => a.RoomTypeId == roomTypeId);
        }
        // Check khuyến mãi PromotionId đã áp dụng cho loại phòng RoomTypeId chưa?
        private async Task<bool> CheckApply(int promotionId, int roomTypeId, int? applyPromotionId = null)
        {
            return await _context.ApplyPromotions.AnyAsync(a => a.PromotionId == promotionId && a.RoomTypeId == roomTypeId
                                                             && (applyPromotionId == null || applyPromotionId != a.ApplyPromotionId));
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
            if (!await CheckPromotionId(dto.PromotionId))
                throw new NotFoundException("Mã khuyến mãi không tồn tại!");
            if (!await CheckRoomTypeId(dto.RoomTypeId))
                throw new NotFoundException("Loại phòng không tồn tại!");
            if (await CheckApply(dto.PromotionId, dto.RoomTypeId))
                throw new ConflictException("Khuyến mãi đã được áp dụng cho loại phòng này. Không thể áp dụng tiếp!");
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
            if (!await CheckPromotionId(dto.PromotionId))
                throw new NotFoundException("Mã khuyến mãi không tồn tại!");
            if (!await CheckRoomTypeId(dto.RoomTypeId))
                throw new NotFoundException("Loại phòng không tồn tại!");
            if (await CheckApply(dto.PromotionId, dto.RoomTypeId, id))
                throw new ConflictException("Khuyến mãi đã được áp dụng cho loại phòng này. Không thể áp dụng tiếp!");
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
