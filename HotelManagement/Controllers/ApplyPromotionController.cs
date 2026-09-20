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
        [HttpPost]
        public async Task<IActionResult> Create(ApplyPromotionCreateDto dto)
        {
            var createApplyPromotion = await _applyPromotionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new { id = createApplyPromotion.ApplyPromotionId },
                                    createApplyPromotion);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ApplyPromotionUpdateDto dto)
        {
            var updateApplyPromotion = await _applyPromotionService.UpdateAsync(id, dto);
            if (updateApplyPromotion is null) return NotFound();
            return NoContent();
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
