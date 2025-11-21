using Microsoft.AspNetCore.Mvc;
using Codemy.FileStorage.Application.Services;
using Codemy.FileStorage.Application.DTOs;


using Codemy.BuildingBlocks.Core;
using Swashbuckle.AspNetCore.Annotations;

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

        [RequestSizeLimit(1_000_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 1_000_000_000)]
        [HttpPost("{type}")]
        public async Task<IActionResult> UploadFile(
            [FromRoute] string type,
            IFormFile file)
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
        [SwaggerOperation(Summary = "Delete file", Description = "Delete a file from Cloudinary by its public ID")]
        public async Task<IActionResult> DeleteFile(
            [SwaggerParameter("The public ID of the file to delete")]
            string publicId)
        {
            await _fileAppService.DeleteFileAsync(publicId);
            return NoContent();
        }
    }
}
