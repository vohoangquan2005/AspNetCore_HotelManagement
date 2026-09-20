using HotelManagement.Data;
using HotelManagement.DTOs.Room;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class RoomService
    {
        private readonly AppDbContext _context;
        public RoomService (AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Room -> RoomResponseDto
        private RoomResponseDto MapToResponseDto(Room room)
        {
            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType?.RoomTypeName,
                Capacity = room.Capacity,
                Location = room.Location,
                PricePerNight = room.PricePerNight,
                Description = room.Description,
                Status = room.Status,
                IsDeleted = room.IsDeleted,
                CreateAt = room.CreateAt,
                UpdateAt = room.UpdateAt
            };
        }
        // Chuyển RoomCreateDto -> Room
        private Room MapToEntity(RoomCreateDto dto)
        {
            return new Room
            {
                RoomName = dto.RoomName,
                RoomTypeId = dto.RoomTypeId,
                Capacity = dto.Capacity,
                Location = dto.Location,
                PricePerNight = dto.PricePerNight,
                Description = dto.Description
            };
        }
        // Cập nhật Room từ RoomUpdateDto
        private void UpdateEntity(Room room, RoomUpdateDto dto)
        {
            room.RoomName = dto.RoomName;
            room.RoomTypeId = dto.RoomTypeId;
            room.Capacity = dto.Capacity;
            room.Location = dto.Location;
            room.PricePerNight = dto.PricePerNight;
            room.Description = dto.Description;
        }
        // Check trùng room name
        private async Task RoomNameExists(string roomName, int? roomId = null) 
        {
            var exists = await _context.Rooms.AnyAsync(r => r.RoomName == roomName
                                                   && (roomId == null || roomId != r.RoomId));
            if (exists)
                throw new ConflictException("Tên phòng đã tồn tại!");
        }
        // Check RoomTypeId đã tồn tại chưa?
        private async Task CheckRoomTypeId(int roomTypeId) 
        {
            var exists = await _context.RoomTypes.AnyAsync(r => r.RoomTypeId == roomTypeId);
            if (!exists)
                throw new NotFoundException("Loại phòng không tồn tại!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all Active(Lấy room còn hoạt động)
        public async Task<List<RoomResponseDto>> GetAllAsync()
        {
            var rooms = await _context.Rooms.Where(r => r.IsDeleted == ActiveEnums.Active)
                                            .Select(r => new RoomResponseDto
                                            {
                                                RoomId = r.RoomId,
                                                RoomName = r.RoomName,
                                                RoomTypeId = r.RoomTypeId,
                                                RoomTypeName = r.RoomType!.RoomTypeName,
                                                Capacity = r.Capacity,
                                                Location = r.Location,
                                                PricePerNight = r.PricePerNight,
                                                Description = r.Description,
                                                Status = r.Status,
                                                IsDeleted = r.IsDeleted,
                                                CreateAt = r.CreateAt,
                                                UpdateAt = r.UpdateAt
                                            }).ToListAsync();
            return rooms;
        }
        // Get all InActive(Lấy room không hoạt động)
        public async Task<List<RoomResponseDto>> GetDeletedAsync()
        {
            var rooms = await _context.Rooms.Where(r => r.IsDeleted == ActiveEnums.Inactive)
                                            .Select(r => new RoomResponseDto
                                            {
                                                RoomId = r.RoomId,
                                                RoomName = r.RoomName,
                                                RoomTypeId = r.RoomTypeId,
                                                RoomTypeName = r.RoomType!.RoomTypeName,
                                                Capacity = r.Capacity,
                                                Location = r.Location,
                                                PricePerNight = r.PricePerNight,
                                                Description = r.Description,
                                                Status = r.Status,
                                                IsDeleted = r.IsDeleted,
                                                CreateAt = r.CreateAt,
                                                UpdateAt = r.UpdateAt
                                            }).ToListAsync();
            return rooms;
        }
        // Get by Id
        public async Task<RoomResponseDto?> GetByIdAsync(int id)
        {
            var room = await _context.Rooms.Include(r => r.RoomType)
                                           .FirstOrDefaultAsync(r => r.RoomId == id);
            if (room is null) return null;
            return MapToResponseDto(room);
        }
        // Create 
        public async Task<RoomResponseDto> CreateAsync(RoomCreateDto dto)
        {
            await RoomNameExists(dto.RoomName!);
            await CheckRoomTypeId(dto.RoomTypeId);
            var room = MapToEntity(dto);
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(room.RoomId))!;
        }
        // Update
        public async Task<RoomResponseDto?> UpdateAsync(int id, RoomUpdateDto dto)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room is null) return null;
            await RoomNameExists(dto.RoomName!, id);
            await CheckRoomTypeId(dto.RoomTypeId);
            UpdateEntity(room, dto);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room is null) return false;
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }
        // Xóa tạm
        public async Task<RoomResponseDto?> SoftDeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room is null) return null;
            room.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
        // Khôi phục (Restore)
        public async Task<RoomResponseDto?> RestoreAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room is null) return null;
            room.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

    }
}
