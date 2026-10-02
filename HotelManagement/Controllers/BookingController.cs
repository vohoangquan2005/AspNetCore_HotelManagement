using HotelManagement.DTOs.Booking;
using HotelManagement.Enums;
using HotelManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var bookings = await _bookingService.GetDeletedAsync();
            return Ok(bookings);
        }
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking is null) return NotFound();
            return Ok(booking);
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim is null)
                return Unauthorized("Không phải tài khoản khách hàng!");
            if (!int.TryParse(customerIdClaim.Value, out var customerId))
                return Unauthorized("Mã khách hàng không hợp lệ!");
            var bookings = await _bookingService.GetMyBookingsAsync(customerId);
            return Ok(bookings);
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("my-bookings/{bookingId:int}")]
        public async Task<IActionResult> GetMyBookingDetail(int bookingId)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim is null)
                return Unauthorized("Không phải tài khoản khách hàng!");
            if (!int.TryParse(customerIdClaim.Value, out var customerId))
                return Unauthorized("Mã khách hàng không hợp lệ!");
            var booking = await _bookingService
                .GetMyBookingDetailAsync(customerId, bookingId);
            if (booking is null)
                return NotFound("Không tìm thấy đặt phòng!");
            return Ok(booking);
        }   
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        public async Task<IActionResult> Create(BookingCreateDto dto)
        {
            var createBooking = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new { id = createBooking.BookingId },
                                    createBooking);
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("online")]
        public async Task<IActionResult> CreateOnline(BookingCreateOnlineDto dto)
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            if (customerIdClaim is null)
                return Unauthorized("Không phải tài khoản khách hàng!");
            if (!int.TryParse(customerIdClaim.Value, out var customerId))
                return Unauthorized("Mã khách hàng không hợp lệ!");
            var booking = await _bookingService.CreateOnlineAsync(dto, customerId);
            return CreatedAtAction(nameof(GetById),
                                   new { id = booking.BookingId },
                                   booking);
        }
        [Authorize(Roles = "Admin, Staff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookingUpdateDto dto)
        {
            var updateBooking = await _bookingService.UpdateAsync(id, dto);
            if (updateBooking is null) return NotFound();
            return Ok(updateBooking);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("{id}/confirm")]
        public async Task<IActionResult> SetStatus(int id, BookingEnums status)
        {
            var booking = await _bookingService.ConfirmAsync(id);
            if (booking is null) return NotFound();
            return Ok(booking);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("{id}/soft-deleted")]
        public async Task<IActionResult> SoftDeleted(int id)
        {
            var deleteBooking = await _bookingService.SoftDeleteAsync(id);
            if (!deleteBooking) return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var restored = await _bookingService.RestoreAsync(id);
            if (!restored) return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("auto-soft-delete-expired")]
        public async Task<IActionResult> AutoSoftDeleteExpired()
        {
            var count = await _bookingService.AutoSoftDeleteExpiredAsync();
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteBooking = await _bookingService.DeleteAsync(id);
            if (!deleteBooking) return NotFound();
            return NoContent();
        }      
    }
}
