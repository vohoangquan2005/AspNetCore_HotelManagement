using HotelManagement.DTOs.Booking;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking is null) return NotFound();
            return Ok(booking);
        }
        [HttpPost]
        public async Task<IActionResult> Create(BookingCreateDto dto)
        {
            var createBooking = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new { id = createBooking.BookingId },
                                    createBooking);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookingUpdateDto dto)
        {
            var updateBooking = await _bookingService.UpdateAsync(id, dto);
            if (updateBooking is null) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteBooking = await _bookingService.DeleteAsync(id);
            if (!deleteBooking) return NotFound();
            return NoContent();
        }
    }
}
