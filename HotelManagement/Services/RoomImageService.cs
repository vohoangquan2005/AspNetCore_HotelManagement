using HotelManagement.Data;
using HotelManagement.DTOs.RoomImage;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Exceptions;

namespace HotelManagement.Services
{
    public class RoomImageService
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;
        public RoomImageService(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }
        // Chuyển RoomImage -> ResponseDto
        private RoomImageResponseDto MapToResponseDto(RoomImage roomImage)
        {
            return new RoomImageResponseDto
            {
                RoomImageId = roomImage.RoomImageId,
                RoomId = roomImage.RoomId,
                RoomName = roomImage.Room?.RoomName,
                ImageUrl = roomImage.ImageUrl,
                IsPrimary = roomImage.IsPrimary,
                DisplayOrder = roomImage.DisplayOrder,
                CreateAt = roomImage.CreateAt,
                UpdateAt = roomImage.UpdateAt
            };
        }
        // Chuyển CreateDto -> RoomImage
        private RoomImage MapToEntity(RoomImageCreateDto dto, string imageUrl)
        {
            return new RoomImage
            {
                RoomId = dto.RoomId,
                ImageUrl = imageUrl,
                IsPrimary = dto.IsPrimary,
                DisplayOrder = dto.DisplayOrder
            };
        }
        // Update RoomImage từ UpdateDto
        private void UpdateEntity(RoomImage roomImage, RoomImageUpdateDto dto, string? imageUrl = null)
        {
            // Chỉ thay ImageUrl nếu có ảnh mới
            if (!string.IsNullOrEmpty(imageUrl))
                roomImage.ImageUrl = imageUrl;
            roomImage.IsPrimary = dto.IsPrimary;
            roomImage.DisplayOrder = dto.DisplayOrder;
        }
        // Check RoomId có tồn tại trong bảng Room không?
        private async Task CheckRoomId(int  roomId)
        {
            var exists = await _context.Rooms.AnyAsync(r => r.RoomId == roomId);
            if (!exists)
                throw new NotFoundException("Mã phòng không hợp lệ!");
        }
        // Cập nhật IsPrimary
        private async Task SetPrimaryImage(int roomId, int? roomImageId = null)
        {
            var primaryImage = await _context.RoomImages
                                             .FirstOrDefaultAsync(ri => ri.RoomId == roomId 
                                                               && ri.IsPrimary == 1
                                                               && (roomImageId == null || ri.RoomImageId != roomImageId));
            if (primaryImage is not null)
                primaryImage.IsPrimary = 0;
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all 
        public async Task<List<RoomImageResponseDto>> GetAllAsync()
        {
            var roomImages = await _context.RoomImages
                                           .OrderBy(i => i.RoomId)
                                                .ThenBy(i => i.DisplayOrder)
                                           .Select(ri => new RoomImageResponseDto
                                           {
                                                RoomImageId = ri.RoomImageId,
                                                RoomId = ri.RoomId,
                                                RoomName = ri.Room!.RoomName,
                                                ImageUrl = ri.ImageUrl,
                                                IsPrimary = ri.IsPrimary,
                                                DisplayOrder = ri.DisplayOrder,
                                                CreateAt = ri.CreateAt,
                                                UpdateAt = ri.UpdateAt
                                           })
                                           .ToListAsync();
            return roomImages;
        }
        // Get by Id
        public async Task<RoomImageResponseDto?> GetByIdAsync(int id)
        {
            var roomImage = await _context.RoomImages
                                          .Include(ri => ri.Room)
                                          .FirstOrDefaultAsync(ri => ri.RoomImageId == id);
            if (roomImage is null) return null;
            return MapToResponseDto(roomImage);
        }
        // Create
        public async Task<RoomImageResponseDto> CreateAsync(RoomImageCreateDto dto)
        {
            await CheckRoomId(dto.RoomId);
            // Nếu tạo ảnh chỉnh -> bỏ ảnh chính cũ
            if (dto.IsPrimary == 1)
                await SetPrimaryImage(dto.RoomId);
            // Upload ảnh lên Cloud
            var imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image);
            var roomImage = MapToEntity(dto, imageUrl);
            await _context.RoomImages.AddAsync(roomImage);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(roomImage.RoomImageId))!;
        }
        // Update
        public async Task<RoomImageResponseDto?> UpdateAsync(int id, RoomImageUpdateDto dto)
        {
            var roomImage = await _context.RoomImages.FindAsync(id);
            if (roomImage is null) return null;
            // Lưu URL ảnh cũ
            var oldImageUrl = roomImage.ImageUrl;
            if (dto.IsPrimary == 1)
                await SetPrimaryImage(roomImage.RoomId, id);
            // Url mới
            string? newImageUrl = null;
            // Nếu cập nhật thành ảnh chính -> bỏ ảnh chính cũ
            string? imageUrl = null;
            if (dto.Image != null && dto.Image.Length > 0)
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.Image);
            UpdateEntity(roomImage, dto, newImageUrl);
            await _context.SaveChangesAsync();
            // Xóa ảnh cũ trên Cloud
            if (!string.IsNullOrWhiteSpace(newImageUrl) && !string.IsNullOrWhiteSpace(oldImageUrl))
                await _cloudinaryService.DeleteImageAsync(oldImageUrl);
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var roomImage = await _context.RoomImages.FindAsync(id);
            if (roomImage is null) return false;
            // Lưu Url cũ
            var imageUrl = roomImage.ImageUrl;
            _context.RoomImages.Remove(roomImage);
            await _context.SaveChangesAsync();
            // 4. Xóa ảnh trên Cloudinary
            if (!string.IsNullOrWhiteSpace(imageUrl))
                await _cloudinaryService.DeleteImageAsync(imageUrl);
            return true;
        }
    }
}
