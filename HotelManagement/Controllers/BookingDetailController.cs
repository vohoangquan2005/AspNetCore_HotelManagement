using HotelManagement.DTOs.BookingDetail;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingDetailController : ControllerBase
    {
        private readonly BookingDetailService _bookingDetailService;
        public BookingDetailController(BookingDetailService bookingDetailService)
        {
            _bookingDetailService = bookingDetailService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookingDetails = await _bookingDetailService.GetAllAsync();
            return Ok(bookingDetails);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingDetailService.GetByIdAsync(id);
            if (booking is null) return NotFound();
            return Ok(booking);
        }
        [HttpPost]
        public async Task<IActionResult> Create(BookingDetailCreateDto dto)
        {
            var createBookingDetail = await _bookingDetailService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new { id = createBookingDetail.BookingDetailId },
                                    createBookingDetail);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookingDetailUpdateDto dto)
        {
            var updateBookingDetail = await _bookingDetailService.UpdateAsync(id, dto);
            if (updateBookingDetail is null) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteBookingDetail = await _bookingDetailService.DeleteAsync(id);
            if (!deleteBookingDetail) return NotFound();
            return NoContent();
        }
    }
}
