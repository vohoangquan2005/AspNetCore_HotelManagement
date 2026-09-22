using HotelManagement.DTOs.AuditLog;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly AuditLogService _auditLogService;
        public AuditLogController(AuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var auditLogs = await _auditLogService.GetAllAsync();
            return Ok(auditLogs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var auditLog = await _auditLogService.GetByIdAsync(id);
            if (auditLog is null)  return NotFound();
            return Ok(auditLog);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AuditLogCreateDto dto)
        {
            var auditLog = await _auditLogService.CreateAsync(dto);
            return CreatedAtAction( nameof(GetById),
                                    new { id = auditLog.AuditLogId },
                                    auditLog);
        }
    }
}