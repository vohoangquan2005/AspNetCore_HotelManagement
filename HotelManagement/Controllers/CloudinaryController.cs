using HotelManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;
        public CloudinaryController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var url = await _cloudinaryService.UploadImageAsync(file);
            return Ok(new {Url = url});
        }
        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteImage(
            [FromQuery] string imageUrl)
        {
            await _cloudinaryService.DeleteImageAsync(imageUrl);
            return Ok();
        }
    }
}
