using HotelManagement.DTOs.FavoriteRoom;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteRoomController : ControllerBase
    {
        private readonly FavoriteRoomService _favoriteRoomService;
        public FavoriteRoomController(FavoriteRoomService favoriteRoomService)
        {
            _favoriteRoomService = favoriteRoomService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var favoriteRooms = await _favoriteRoomService.GetAllAsync();
            return Ok(favoriteRooms);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var favoriteRoom = await _favoriteRoomService.GetByIdAsync(id);
            if (favoriteRoom is null) return NotFound();
            return Ok(favoriteRoom);
        }
        [HttpPost]
        public async Task<IActionResult> Create(FavoriteRoomCreateDto dto)
        {
            var favoriteRoom = await _favoriteRoomService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = favoriteRoom.FavoriteRoomId },
                                   favoriteRoom);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _favoriteRoomService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
