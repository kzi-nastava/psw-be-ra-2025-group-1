using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Route("api/upload")]
    [Authorize]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly string _rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public UploadController()
        {
            if (!Directory.Exists(_rootFolder))
                Directory.CreateDirectory(_rootFolder);
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var imageFolder = Path.Combine(_rootFolder, "images");
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Allowed: .jpg, .jpeg, .png");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(imageFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var fileUrl = $"{baseUrl}/images/{fileName}";

            return Ok(new { imageUrl = fileUrl });
        }

        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            var videoFolder = Path.Combine(_rootFolder, "videos");
            if (!Directory.Exists(videoFolder))
                Directory.CreateDirectory(videoFolder);

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedExtensions = new[] { ".mp4", ".webm", ".mov" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Allowed: .mp4, .webm, .mov");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(videoFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var fileUrl = $"{baseUrl}/videos/{fileName}";

            return Ok(new { videoUrl = fileUrl });
        }

    }
}
