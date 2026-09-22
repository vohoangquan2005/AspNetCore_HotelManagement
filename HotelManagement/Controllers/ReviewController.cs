using HotelManagement.DTOs.Review;
using HotelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;
        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewService.GetAllAsync();
            return Ok(reviews);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review is null) return NotFound();
            return Ok(review);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ReviewCreateDto dto)
        {
            var review = await _reviewService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                                   new { id = review.ReviewId },
                                   review);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ReviewUpdateDto dto)
        {
            var review = await _reviewService.UpdateAsync(id, dto);
            if (review is null) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}