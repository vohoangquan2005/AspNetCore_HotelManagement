using HotelManagement.Data;
using HotelManagement.DTOs.AuditLog;
using HotelManagement.Exceptions;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class AuditLogService
    {
        private readonly AppDbContext _context;
        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }
        // Chuyển AuditLog -> ResponseDto
        private AuditLogResponseDto MapToResponseDto(AuditLog auditLog)
        {
            return new AuditLogResponseDto
            {
                AuditLogId = auditLog.AuditLogId,
                UserId = auditLog.UserId,
                UserName = auditLog.User?.FullName,
                Action = auditLog.Action,
                TableName = auditLog.TableName,
                RecordId = auditLog.RecordId,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue,
                CreateAt = auditLog.CreateAt
            };
        }
        // Chuyển CreateDto -> AuditLog
        private AuditLog MapToEntity(AuditLogCreateDto dto)
        {
            return new AuditLog
            {
                UserId = dto.UserId,
                Action = dto.Action,
                TableName = dto.TableName,
                RecordId = dto.RecordId,
                OldValue = dto.OldValue,
                NewValue = dto.NewValue
            };
        }
        // Check UserId có tồn tại trong bảng User không?
        private async Task CheckUserId(int? userId)
        {
            if (userId is null) return;
            var exists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!exists)
                throw new NotFoundException("Mã người dùng không hợp lệ!");
        }
        // ===========================================================
        // =                           CRUD                          =
        // ===========================================================
        // Get all 
        public async Task<List<AuditLogResponseDto>> GetAllAsync()
        {
            var auditLogs = await _context.AuditLogs.Include(a => a.User)
                                                    .Select(a => new AuditLogResponseDto
                                                    {
                                                        AuditLogId = a.AuditLogId,
                                                        UserId = a.UserId,
                                                        UserName = a.User!.FullName,
                                                        Action = a.Action,
                                                        TableName = a.TableName,
                                                        RecordId = a.RecordId,
                                                        OldValue = a.OldValue,
                                                        NewValue = a.NewValue,
                                                        CreateAt = a.CreateAt
                                                    })
                                                    .ToListAsync();
            return auditLogs;
        }
        // Get by Id
        public async Task<AuditLogResponseDto?> GetByIdAsync(int id)
        {
            var auditLog = await _context.AuditLogs
                                         .Include(a => a.User)
                                         .FirstOrDefaultAsync(a => a.AuditLogId == id);
            if (auditLog is null) return null;
            return MapToResponseDto(auditLog);
        }
        // Create
        public async Task<AuditLogResponseDto> CreateAsync(AuditLogCreateDto dto)
        {
            await CheckUserId(dto.UserId);
            var auditLog = MapToEntity(dto);
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(auditLog.AuditLogId)
                   ?? throw new NotFoundException("Không tìm thấy nhật ký vừa tạo!");
        }
    }
}