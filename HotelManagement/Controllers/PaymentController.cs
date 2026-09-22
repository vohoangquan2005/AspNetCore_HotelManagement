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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PaymentUpdateDto dto)
        {
            var payment = await _paymentService.UpdateAsync(id, dto);
            if (payment is null) return NotFound();
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
