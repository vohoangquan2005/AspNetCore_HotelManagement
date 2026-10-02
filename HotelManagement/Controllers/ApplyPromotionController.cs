using HotelManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ApplyPromotionController : ControllerBase
    {
        private readonly ApplyPromotionService _applyPromotionService;
        public ApplyPromotionController(ApplyPromotionService applyPromotionService)
        {
            _applyPromotionService = applyPromotionService;
        }
        [Authorize(Roles = "Admin,Staff, Customer")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var applies = await _applyPromotionService.GetAllAsync();
            return Ok(applies);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var apply = await _applyPromotionService.GetByIdAsync(id);
            if (apply is null) return NotFound();
            return Ok(apply);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteApplyPromotion = await _applyPromotionService.DeleteAsync(id);
            if (!deleteApplyPromotion) return NotFound();
            return NoContent();
        }
    }
}
