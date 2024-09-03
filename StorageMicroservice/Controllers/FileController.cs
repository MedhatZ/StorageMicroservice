// Controllers/FileController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageMicroservice.Services;

namespace StorageMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected.");

            var metadata = await _fileService.SaveFileAsync(file);
            return Ok(metadata);
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var fileStream = await _fileService.GetFileAsync(fileId);
            if (fileStream == null) return NotFound();

            return File(fileStream, "application/octet-stream");
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            await _fileService.DeleteFileAsync(fileId);
            return NoContent();
        }

        [HttpGet("{fileId}/metadata")]
        public async Task<IActionResult> GetMetadata(string fileId)
        {
            var metadata = await _fileService.GetMetadataAsync(fileId);
            if (metadata == null) return NotFound();

            return Ok(metadata);
        }
    }
}
