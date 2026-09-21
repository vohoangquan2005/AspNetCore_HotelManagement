using HotelManagement.DTOs.BookingService;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingServiceController : ControllerBase
    {
        private readonly BookingServiceService _bookingService;
        public BookingServiceController(BookingServiceService bookingService)
        {
            _bookingService = bookingService;
        }
        // Get all
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookingServices = await _bookingService.GetAllAsync();
            return Ok(bookingServices);
        }
        // Get by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bookingService = await _bookingService.GetByIdAsync(id);
            if (bookingService is null)
                return NotFound();
            return Ok(bookingService);
        }
        // Create
        [HttpPost]
        public async Task<IActionResult> Create(BookingServiceCreateDto dto)
        {
            var bookingService = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = bookingService.BookingServiceId },
                                   bookingService);
        }
        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookingServiceUpdateDto dto)
        {
            var bookingService = await _bookingService.UpdateAsync(id, dto);
            if (bookingService is null) return NotFound();
            return NoContent();
        }
        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
