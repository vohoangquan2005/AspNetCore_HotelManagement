using HotelManagement.Data;
using HotelManagement.DTOs.RoomType;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTypeController : ControllerBase
    {
        private readonly RoomTypeService _roomTypeService;
        public RoomTypeController(RoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }
        [HttpGet]   // Lấy danh sách roomtype đang hoạt động (Action)
        public async Task<IActionResult> GetAll()
        {
            var roomTypes = await _roomTypeService.GetAllAsync();
            return Ok(roomTypes);
        }
        [HttpGet("deleted")]    // Lấy danh sách roomtype đã xóa (Inaction)
        public async Task<IActionResult> GetDeleted()
        {
            var roomTypes = await _roomTypeService.GetDeletedAsync();
            return Ok(roomTypes);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var roomType = await _roomTypeService.GetByIdAsync(id);
            if (roomType is null) return NotFound();
            return Ok(roomType);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoomTypeCreateDto dto)
        {
            var createRoomType = await _roomTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new { id = createRoomType.RoomTypeId },
                                    createRoomType);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RoomTypeUpdateDto dto)
        {
            var updateRoomType = await _roomTypeService.UpdateAsync(id, dto);
            if (updateRoomType is null) return NotFound();
            return NoContent();
        }
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleteRoomType = await _roomTypeService.SoftDeleteAsync(id);

            if (deleteRoomType is null)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteRoomType = await _roomTypeService.DeleteAsync(id);
            if (deleteRoomType is null) return NotFound();
            return NoContent();
        }
    }
}
