using HotelManagement.DTOs.Cancellation;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CancellationController : ControllerBase
    {
        private readonly CancellationService _cancellationService;

        public CancellationController(CancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cancellations = await _cancellationService.GetAllAsync();
            return Ok(cancellations);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cancellation = await _cancellationService.GetByIdAsync(id);
            if (cancellation is null) return NotFound();
            return Ok(cancellation);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CancellationCreateDto dto)
        {
            var cancellation = await _cancellationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                  new { id = cancellation.CancellationId },
                                  cancellation);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CancellationUpdateDto dto)
        {
            var cancellation = await _cancellationService.UpdateAsync(id, dto);
            if (cancellation is null) return NotFound();
            return NoContent();
        }
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _cancellationService.ConfirmAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _cancellationService.RejectAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cancellationService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
