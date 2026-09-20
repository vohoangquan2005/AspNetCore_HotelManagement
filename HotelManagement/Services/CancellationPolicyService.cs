using HotelManagement.Data;
using HotelManagement.DTOs.CancallationPolicy;
using HotelManagement.Enums;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class CancellationPolicyService
    {
        private readonly AppDbContext _context;
        public CancellationPolicyService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển CancellationPolicy -> ResponseDto
        private CancellationPolicyResponseDto MapToResponseDto(CancellationPolicy policy)
        {
            return new CancellationPolicyResponseDto
            {
                CancellationPolicyId = policy.CancellationPolicyId,
                PolicyName = policy.PolicyName,
                CancellationHours = policy.CancellationHours,
                RefundPercentage = policy.RefundPercentage,
                Description = policy.Description,
                IsDeleted = policy.IsDeleted,
                CreateAt = policy.CreateAt,
                UpdateAt = policy.UpdateAt
            };
        }
        // Chuyển CancellationPolicyCreateDto -> CancellationPolicy
        private CancellationPolicy MapToEntity(CancellationPolicyCreateDto dto)
        {
            return new CancellationPolicy
            {
                PolicyName = dto.PolicyName,
                CancellationHours = dto.CancellationHours,
                RefundPercentage = dto.RefundPercentage,
                Description = dto.Description,
            };
        }
        // Update CancellationPolicy từ 
        private void UpdateEntity(CancellationPolicy policy, CancellationPolicyUpdateDto dto)
        {
            policy.PolicyName = dto.PolicyName;
            policy.CancellationHours = dto.CancellationHours;
            policy.RefundPercentage = dto.RefundPercentage;
            policy.Description = dto.Description;
        }

        // Check trùng tên chính sách
        private async Task<bool> PolicyNameExists(string policyName, int? cancellationPolicyId = null)
        {
            return await _context.CancellationPolicies.AnyAsync(cp => cp.PolicyName == policyName
                                                && (cancellationPolicyId == null || cancellationPolicyId != cp.CancellationPolicyId));
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all Active(Lấy cancellation policy còn hoạt động)
        public async Task<List<CancellationPolicyResponseDto>> GetAllAsync()
        {
            var policies = await _context.CancellationPolicies.Where(cp => cp.IsDeleted == ActiveEnums.Active)
                                            .Select(cp => new CancellationPolicyResponseDto
                                            {
                                                CancellationPolicyId = cp.CancellationPolicyId,
                                                PolicyName = cp.PolicyName,
                                                CancellationHours = cp.CancellationHours,
                                                RefundPercentage = cp.RefundPercentage,
                                                Description = cp.Description,
                                                IsDeleted = cp.IsDeleted,
                                                CreateAt = cp.CreateAt,
                                                UpdateAt = cp.UpdateAt
                                            }).ToListAsync();
            return policies;
        }
        // Get all InActive(Lấy cancellation policy không hoạt động)
        public async Task<List<CancellationPolicyResponseDto>> GetDeletedAsync()
        {
            var policies = await _context.CancellationPolicies.Where(cp => cp.IsDeleted == ActiveEnums.Inactive)
                                            .Select(cp => new CancellationPolicyResponseDto
                                            {
                                                CancellationPolicyId = cp.CancellationPolicyId,
                                                PolicyName = cp.PolicyName,
                                                CancellationHours = cp.CancellationHours,
                                                RefundPercentage = cp.RefundPercentage,
                                                Description = cp.Description,
                                                IsDeleted = cp.IsDeleted,
                                                CreateAt = cp.CreateAt,
                                                UpdateAt = cp.UpdateAt
                                            }).ToListAsync();
            return policies;
        }
        // Get by Id
        public async Task<CancellationPolicyResponseDto?> GetByIdAsync(int id)
        {
            var policy = await _context.CancellationPolicies.FindAsync(id);
            if (policy is null) return null;
            return MapToResponseDto(policy);
        }
        // Create
        public async Task<CancellationPolicyResponseDto> CreateAsync(CancellationPolicyCreateDto dto)
        {
            if (await PolicyNameExists(dto.PolicyName!))
                throw new ConflictException("Tên chính sách đã tồn tại!");
            var policy = MapToEntity(dto);
            await _context.CancellationPolicies.AddAsync(policy);
            await _context.SaveChangesAsync();
            return MapToResponseDto(policy);
        }
        // Update
        public async Task<CancellationPolicyResponseDto?> UpdateAsync(int id, CancellationPolicyUpdateDto dto)
        {
            var policy = await _context.CancellationPolicies.FindAsync(id);
            if (policy is null) return null;
            if (await PolicyNameExists(dto.PolicyName!, id))
                throw new ConflictException("Tên chính sách đã tồn tại!");
            UpdateEntity(policy, dto);
            await _context.SaveChangesAsync();
            return MapToResponseDto(policy);
        }
        // Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var policy = await _context.CancellationPolicies.FindAsync(id);
            if (policy is null) return false;
            _context.CancellationPolicies.Remove(policy);
            await _context.SaveChangesAsync();
            return true;
        }
        // Xóa tạm
        public async Task<CancellationPolicyResponseDto?> SoftDeleteAsync(int id)
        {
            var policy = await _context.CancellationPolicies.FindAsync(id);
            if (policy is null) return null;
            policy.IsDeleted = ActiveEnums.Inactive;
            await _context.SaveChangesAsync();
            return MapToResponseDto(policy);
        }
        // Khôi phục (Restore)
        public async Task<CancellationPolicyResponseDto?> RestoreAsync(int id)
        {
            var policy = await _context.CancellationPolicies.FirstOrDefaultAsync(u => u.CancellationPolicyId == id);
            if (policy is null) return null;
            policy.IsDeleted = ActiveEnums.Active;
            await _context.SaveChangesAsync();
            return MapToResponseDto(policy);
        }
    }
}
