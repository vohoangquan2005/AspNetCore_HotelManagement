using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.DTOs.Role;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpGet]   // Lấy danh sách Role đang hoạt động (Action)
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }
        [HttpGet("deleted")]    // Lấy danh sách Role đã bị xóa (Inaction)
        public async Task<IActionResult> GetDeleted()
        {
            var roles = await _roleService.GetDeletedAsync();
            return Ok(roles);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role is null) return NotFound();
            return Ok(role);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleCreateDto dto)
        {
            var createRole = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new {id = createRole.RoleId}, 
                                    createRole);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RoleUpdateDto dto)
        {
            var updateRole = await _roleService.UpdateAsync(id, dto);
            if (updateRole is null) return NotFound();
            return NoContent();
        }
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleteRole = await _roleService.SoftDeleteAsync(id);

            if (deleteRole is null)
                return NotFound();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteRole = await _roleService.DeleteAsync(id);
            if (deleteRole is null) return NotFound();
            return NoContent();
        }
    }
}
