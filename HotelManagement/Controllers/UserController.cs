using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.DTOs.User;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpGet]   // Lấy danh sách user đang hoạt động (Active)
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        [HttpGet("deleted")]    // Lấy danh sách user đã xóa (Inactive)
        public async Task<IActionResult> GetDeleted()
        {
            var users = await _userService.GetDeletedAsync();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null) return NotFound();
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto dto)
        {
            var createUser = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new { id = createUser.UserId },
                                    createUser);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto dto)
        {
            var updateUser = await _userService.UpdateAsync(id, dto);
            if (updateUser is null) return NotFound();
            return NoContent();
        }
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleteUser = await _userService.SoftDeleteAsync(id);

            if (deleteUser is null)
                return NotFound();

            return NoContent();
        }
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var restoreUser = await _userService.RestoreAsync(id);
            if (restoreUser is null)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteUser = await _userService.DeleteAsync(id);
            if (!deleteUser) return NotFound();
            return NoContent();
        }
    }
}
