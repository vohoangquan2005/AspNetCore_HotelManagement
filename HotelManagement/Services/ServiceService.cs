using HotelManagement.Data;
using HotelManagement.DTOs.Service;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class ServiceService
    {
        private readonly AppDbContext _context;
        public ServiceService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển Service -> Response Dto
        private ServiceResponseDto MapToResponseDto(Service service)
        {
            return new ServiceResponseDto
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                UnitPrice = service.UnitPrice,
                Unit = service.Unit,
                Description = service.Description,
                IsDeleted = service.IsDeleted,
                CreateAt = service.CreateAt,
                UpdateAt = service.UpdateAt
            };
        }
        // Chuyển CreateDto -> Service
        private Service MapToEntity(ServiceCreateDto dto)
        {
            return new Service
            {
                ServiceName = dto.ServiceName,
                UnitPrice = dto.UnitPrice,
                Unit = dto.Unit,
                Description = dto.Description
            };
        }
        // Update Service từ ServiceUpdateDto
        private void UpdateEntity(Service service, ServiceUpdateDto dto)
        {
            service.ServiceName = dto.ServiceName;
            service.UnitPrice = dto.UnitPrice;
            service.Unit = dto.Unit;
            service.Description = dto.Description;
        }
        // Check trùng ServiceName
        private async Task<bool> ServiceNameExists(string serviceName, int? serviceId = null)
        {
            return await _context.Services.AnyAsync(s => s.ServiceName == serviceName
                                                  && (serviceId == null || serviceId != s.ServiceId));
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all Active(Lấy service còn hoạt động)
        public async Task<List<ServiceResponseDto>> GetAllAsync()
        {
            var services = await _context.Services.Where(s => s.IsDeleted == ActiveEnums.Active)
                                            .Select(s => new ServiceResponseDto
                                            {
                                                ServiceId = s.ServiceId,
                                                ServiceName = s.ServiceName,
                                                UnitPrice = s.UnitPrice,
                                                Unit = s.Unit,
                                                Description = s.Description,
                                                IsDeleted = s.IsDeleted,
                                                CreateAt = s.CreateAt,
                                                UpdateAt = s.UpdateAt
                                            }).ToListAsync();
            return services;
        }
        // Get all InActive(Lấy service không hoạt động)
        public async Task<List<ServiceResponseDto>> GetDeletedAsync()
        {
            var services = await _context.Services.Where(s => s.IsDeleted == ActiveEnums.Inactive)
                                            .Select(s => new ServiceResponseDto
                                            {
                                                ServiceId = s.ServiceId,
                                                ServiceName = s.ServiceName,
                                                UnitPrice = s.UnitPrice,
                                                Unit = s.Unit,
                                                Description = s.Description,
                                                IsDeleted = s.IsDeleted,
                                                CreateAt = s.CreateAt,
                                                UpdateAt = s.UpdateAt
                                            }).ToListAsync();
            return services;
        }
        // Get by Id
        public async Task<ServiceResponseDto?> GetByIdAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service is null) return null;
            return MapToResponseDto(service);
        }
        // Create 
        public async Task<ServiceResponseDto> CreateAsync(ServiceCreateDto dto)
        {
            if (await ServiceNameExists(dto.ServiceName!))
                throw new ConflictException("Tên dịch vụ đã tồn tại!");
            var service = MapToEntity(dto);
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return MapToResponseDto(service);
        }
        // Update
        public async Task<ServiceResponseDto?> UpdateAsync(int id, ServiceUpdateDto dto)
        {
            var service = await _context.Services.FindAsync(id);
            if (service is null) return null;
            if (await ServiceNameExists(dto.ServiceName!, id))
                throw new ConflictException("Tên dịch vụ đã tồn tại!");
            UpdateEntity(service, dto);
            await _context.SaveChangesAsync();
            return MapToResponseDto(service);
        }
        // Delete
        public async Task<ServiceResponseDto?> DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service is null) return null;
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return MapToResponseDto(service);
        }
        // Xóa tạm
        public async Task<ServiceResponseDto?> SoftDeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service is null) return null;
            service.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return MapToResponseDto(service);
        }
        // Khôi phục (Restore)
        public async Task<ServiceResponseDto?> RestoreAsync(int id)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == id);
            if (service is null) return null;
            service.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return MapToResponseDto(service);
        }
    }
}
