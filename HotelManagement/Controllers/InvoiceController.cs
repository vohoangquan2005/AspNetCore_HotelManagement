using HotelManagement.DTOs.Invoice;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;
        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return Ok(invoices);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice is null) return NotFound();
            return Ok(invoice);
        }
        [HttpPost]
        public async Task<IActionResult> Create(InvoiceCreateDto dto)
        {
            var createInvoice = await _invoiceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = createInvoice.InvoiceId },
                                   createInvoice);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteInvoice = await _invoiceService.DeleteAsync(id);
            if (!deleteInvoice) return NotFound();
            return NoContent();
        }
    }
}
