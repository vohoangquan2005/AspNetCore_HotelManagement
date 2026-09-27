using HotelManagement.DTOs.Payment;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment is null) return NotFound();
            return Ok(payment);
        }
        [HttpPost]
        public async Task<IActionResult> Create(PaymentCreateDto dto)
        {
            var payment = await _paymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = payment.PaymentId },
                                   payment);
        }
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _paymentService.ConfirmAsync(id);
            if (result == false) return NotFound();
            return NoContent();
        }
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _paymentService.RejectAsync(id);
            if (result == false) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
