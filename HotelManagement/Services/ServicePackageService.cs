using HotelManagement.Data;
using HotelManagement.DTOs.ServicePackage;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class ServicePackageService
    {
        private readonly AppDbContext _context;
        public ServicePackageService(AppDbContext context) 
        { 
            _context = context; 
        }
        private ServicePackageResponseDto MapToResponseDto(ServicePackage package)
        {
            return new ServicePackageResponseDto
            {
                ServicePackageId = package.ServicePackageId,
                PackageName = package.PackageName,
                PackagePrice = package.PackagePrice,
                Description = package.Description,
                IsDeleted = package.IsDeleted,
                CreateAt = package.CreateAt,
                UpdateAt = package.UpdateAt
            };
        }
        // Chuyển CreateDto -> ServicePackage
        private ServicePackage MapToEntity(ServicePackageCreateDto dto)
        {
            return new ServicePackage
            {
                PackageName = dto.PackageName,
                PackagePrice = dto.PackagePrice,
                Description = dto.Description
            };
        }
        // Update Service Package từ ServicePackageUpdateDto
        private void UpdateEntity(ServicePackage package, ServicePackageUpdateDto dto)
        {
            package.PackageName = dto.PackageName;
            package.PackagePrice = dto.PackagePrice;
            package.Description = dto.Description;
        }
        // Check trùng ServiceName
        private async Task PackageNameExists(string packageName, int? servicePackageId = null)
        {
            var exists = await _context.ServicePackages.AnyAsync(s => s.PackageName == packageName
                                                  && (servicePackageId == null || servicePackageId != s.ServicePackageId));
            if (exists)
                    throw new ConflictException("Tên gói dịch vụ đã tồn tại!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all Active(Lấy service package còn hoạt động)
        public async Task<List<ServicePackageResponseDto>> GetAllAsync()
        {
            var packages = await _context.ServicePackages.Where(sp => sp.IsDeleted == ActiveEnums.Active)
                                            .Select(sp => new ServicePackageResponseDto
                                            {
                                                ServicePackageId = sp.ServicePackageId,
                                                PackageName = sp.PackageName,
                                                PackagePrice = sp.PackagePrice,
                                                Description = sp.Description,
                                                IsDeleted = sp.IsDeleted,
                                                CreateAt = sp.CreateAt,
                                                UpdateAt = sp.UpdateAt
                                            }).ToListAsync();
            return packages;
        }
        // Get all InActive(Lấy service package không hoạt động)
        public async Task<List<ServicePackageResponseDto>> GetDeletedAsync()
        {
            var packages = await _context.ServicePackages.Where(sp => sp.IsDeleted == ActiveEnums.Inactive)
                                            .Select(sp => new ServicePackageResponseDto
                                            {
                                                ServicePackageId = sp.ServicePackageId,
                                                PackageName = sp.PackageName,
                                                PackagePrice = sp.PackagePrice,
                                                Description = sp.Description,
                                                IsDeleted = sp.IsDeleted,
                                                CreateAt = sp.CreateAt,
                                                UpdateAt = sp.UpdateAt
                                            }).ToListAsync();
            return packages;
        }
        // Get by Id
        public async Task<ServicePackageResponseDto?> GetByIdAsync(int id)
        {
            var package = await _context.ServicePackages.FindAsync(id);
            if (package is null) return null;
            return MapToResponseDto(package);
        }
        // Create 
        public async Task<ServicePackageResponseDto> CreateAsync(ServicePackageCreateDto dto)
        {
            await PackageNameExists(dto.PackageName!);
            var package = MapToEntity(dto);
            await _context.ServicePackages.AddAsync(package);
            await _context.SaveChangesAsync();
            return MapToResponseDto(package);
        }
        // Update
        public async Task<ServicePackageResponseDto?> UpdateAsync(int id, ServicePackageUpdateDto dto)
        {
            var package = await _context.ServicePackages.FindAsync(id);
            if (package is null) return null;
            await PackageNameExists(dto.PackageName!, id);
            UpdateEntity(package, dto);
            await _context.SaveChangesAsync();
            return MapToResponseDto(package);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var package = await _context.ServicePackages.FindAsync(id);
            if (package is null) return false;
            _context.ServicePackages.Remove(package);
            await _context.SaveChangesAsync();
            return true;
        }
        // Xóa tạm
        public async Task<ServicePackageResponseDto?> SoftDeleteAsync(int id)
        {
            var package = await _context.ServicePackages.FindAsync(id);
            if (package is null) return null;
            package.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return MapToResponseDto(package);
        }
        // Khôi phục (Restore)
        public async Task<ServicePackageResponseDto?> RestoreAsync(int id)
        {
            var package = await _context.ServicePackages.FindAsync(id);
            if (package is null) return null;
            package.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return MapToResponseDto(package);
        }
    }
}
