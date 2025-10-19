using Microsoft.AspNetCore.Mvc;
using Codemy.FileStorage.Application.Services;
using Codemy.FileStorage.Application.DTOs;


using Codemy.BuildingBlocks.Core;

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
            if (file == null)
                return this.BadRequestResponse("No file uploaded.");

            var result = await _fileAppService.UploadFileAsync(file, type);
            return this.OkResponse(new FileUploadResponse
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
