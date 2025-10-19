using Microsoft.AspNetCore.Mvc;
using Codemy.FileStorage.Application.Services;
using Codemy.FileStorage.API.DTOs;

namespace Codemy.FileStorage.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly FileStorageAppService _fileAppService;

        public FileController(FileStorageAppService fileAppService)
        {
            _fileAppService = fileAppService;
        }

        [HttpPost("{type}")]
        public async Task<IActionResult> UploadFile([FromRoute] string type, IFormFile file)
        {
            var result = await _fileAppService.UploadFileAsync(file, type);
            return Ok(new FileUploadResponse
            {
                Url = result.Url,
                PublicId = result.PublicId,
                Type = result.Type
            });
        }

        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteFile(string publicId)
        {
            await _fileAppService.DeleteFileAsync(publicId);
            return NoContent();
        }
    }
}
