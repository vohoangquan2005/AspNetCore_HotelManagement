using HotelManagement.Data;
using HotelManagement.DTOs.AuditLog;
using HotelManagement.Enums;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Services
{
    public class AuditLogService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public AuditLogService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        // Ghi AuditLog
        public async Task LogAsync(AuditLogEnums action,
                                   string tableName,
                                   int recordId,
                                   string? oldValue = null,
                                   string? newValue = null)
        {
            var auditLog = new AuditLog
                                    {
                                        UserId = _currentUserService.UserId,
                                        Action = action,
                                        TableName = tableName,
                                        RecordId = recordId,
                                        OldValue = oldValue,
                                        NewValue = newValue,
                                        CreateAt = DateTime.Now
                                    };
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }
        // Lấy toàn bộ lịch sử
        public async Task<List<AuditLogResponseDto>> GetAllAsync()
        {
            return await _context.AuditLogs.Include(a => a.User)
                                           .OrderByDescending(a => a.CreateAt)
                                           .Select(a => new AuditLogResponseDto
                                           {
                                                AuditLogId = a.AuditLogId,
                                                UserId = a.UserId,
                                                UserName = a.User != null
                                                                  ? a.User.FullName
                                                                  : null,
                                                Action = a.Action,
                                                TableName = a.TableName,
                                                RecordId = a.RecordId,
                                                OldValue = a.OldValue,
                                                NewValue = a.NewValue,
                                                CreateAt = a.CreateAt
                                           })
                                           .ToListAsync();
        }
    }
}