using FoodOrder.Infrastructure.Services.CloudinaryServices;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public CloudinaryController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadRequest uploadRequest)
        {
            try
            {
                var result = await _cloudinaryService.UploadFileAsync(uploadRequest.File, uploadRequest.Id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred", detail = ex.Message });
            }
        }

        [HttpPost("upload-batch")]
        public async Task<IActionResult> UploadBatchAsync([FromForm] List<IFormFile> files)
        {
            try
            {
                var results = await _cloudinaryService.UploadBatchAsync(files);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Batch upload failed", detail = ex.Message });
            }
        }

        [HttpDelete("{*id}")]
        public async Task<IActionResult> DeleteFile(string id)
        {
            try
            {
                var success = await _cloudinaryService.DeleteFileAsync(id);
                if (success)
                    return Ok(new { message = "File deleted successfully" });
                else
                    return NotFound(new { error = "File not found or could not be deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Deletion failed", detail = ex.Message });
            }
        }

        public class UploadRequest
        {
            public string Id { get; set; } = null;
            public IFormFile? File { get; set; }
        }
    }
}
