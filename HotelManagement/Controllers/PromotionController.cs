using HotelManagement.DTOs.Promotion;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly PromotionService _promotionService;
        public PromotionController(PromotionService promotionService)
        {
            _promotionService = promotionService;
        }
        [HttpGet]   // Lấy danh sách promotion đang hoạt động (Active)
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _promotionService.GetAllAsync();
            return Ok(promotions);
        }
        [HttpGet("deleted")]    // Lấy danh sách promotion đã xóa (Inactive)
        public async Task<IActionResult> GetDeleted()
        {
            var promotions = await _promotionService.GetDeletedAsync();
            return Ok(promotions);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion is null) return NotFound();
            return Ok(promotion);
        }
        [HttpPost]
        public async Task<IActionResult> Create(PromotionCreateDto dto)
        {
            var createPromotion = await _promotionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                    new { id = createPromotion.PromotionId },
                                    createPromotion);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PromotionUpdateDto dto)
        {
            var updatePromotion = await _promotionService.UpdateAsync(id, dto);
            if (updatePromotion is null) return NotFound();
            return NoContent();
        }
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deletePromotion = await _promotionService.SoftDeleteAsync(id);
            if (deletePromotion is null)
                return NotFound();
            return NoContent();
        }
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var restorePromotion = await _promotionService.RestoreAsync(id);
            if (restorePromotion is null)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletePromotion = await _promotionService.DeleteAsync(id);
            if (deletePromotion is null) return NotFound();
            return NoContent();
        }
    }
}
