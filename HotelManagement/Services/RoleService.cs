using HotelManagement.Data;
using HotelManagement.DTOs.Role;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class RoleService
    {
        private readonly AppDbContext _context;
        public RoleService(AppDbContext context)
        {
            _context = context;
        }
        // Hàm chuyển Role -> RoleResponseDto
        private RoleResponseDto MapToResponseDto(Role role)
        {
            return new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                CreateAt = role.CreateAt,
                UpdateAt = role.UpdateAt
            };
        }

        // CRUD
        public async Task<List<RoleResponseDto>> GetAllAsync()
        {
            var roles = await _context.Roles.Select(r => new RoleResponseDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                Description = r.Description,
                CreateAt = r.CreateAt,
                UpdateAt = r.UpdateAt
            }).ToListAsync();
            return roles;
        }
        public async Task<RoleResponseDto?> GetByIdAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null)   return null;
            return MapToResponseDto(role);
        }
        public async Task<RoleResponseDto> CreateAsync(RoleCreateDto dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName,
                Description = dto.Description
            };
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return MapToResponseDto(role);
        }
        public async Task<RoleResponseDto?> UpdateAsync(int id, RoleUpdateDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null) return null;
            role.RoleName = dto.RoleName;
            role.Description = dto.Description;
            await _context.SaveChangesAsync();
            return MapToResponseDto(role);
        }
        public async Task<RoleResponseDto?> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null) return null;
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return MapToResponseDto(role);
        }
    }
}
