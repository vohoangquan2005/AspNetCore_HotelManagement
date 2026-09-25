using HotelManagement.DTOs.ApplyPromotion;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplyPromotionController : ControllerBase
    {
        private readonly ApplyPromotionService _applyPromotionService;
        public ApplyPromotionController(ApplyPromotionService applyPromotionService)
        {
            _applyPromotionService = applyPromotionService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var applies = await _applyPromotionService.GetAllAsync();
            return Ok(applies);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var apply = await _applyPromotionService.GetByIdAsync(id);
            if (apply is null) return NotFound();
            return Ok(apply);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteApplyPromotion = await _applyPromotionService.DeleteAsync(id);
            if (!deleteApplyPromotion) return NotFound();
            return NoContent();
        }
    }
}
