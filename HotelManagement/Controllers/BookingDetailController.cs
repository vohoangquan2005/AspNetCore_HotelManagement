using HotelManagement.DTOs.BookingDetail;
using HotelManagement.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookingDetails = await _bookingDetailService.GetAllAsync();
            return Ok(bookingDetails);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingDetailService.GetByIdAsync(id);
            if (booking is null) return NotFound();
            return Ok(booking);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookingDetailUpdateDto dto)
        {
            var updateBookingDetail = await _bookingDetailService.UpdateAsync(id, dto);
            if (updateBookingDetail is null) return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}/checkin")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var checkIn = await _bookingDetailService.CheckInAsync(id);
            if (!checkIn) return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}/checkout")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var checkOut = await _bookingDetailService.CheckOutAsync(id);
            if (!checkOut) return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteBookingDetail = await _bookingDetailService.DeleteAsync(id);
            if (!deleteBookingDetail) return NotFound();
            return NoContent();
        }
    }
}
