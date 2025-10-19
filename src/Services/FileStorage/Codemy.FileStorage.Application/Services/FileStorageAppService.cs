using Codemy.FileStorage.Application.Interfaces;
using Codemy.FileStorage.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Codemy.FileStorage.Application.Services
{
    public class FileStorageAppService
    {
        private readonly IFileService _fileService;

        public FileStorageAppService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<FileUploadResponse> UploadFileAsync(IFormFile file, string type)
        {
            return type switch
            {
                "image" => await _fileService.UploadImageAsync(file),
                "video" => await _fileService.UploadVideoAsync(file),
                "document" => await _fileService.UploadDocumentAsync(file),
                _ => throw new ArgumentException("Unsupported file type")
            };
        }

        public async Task DeleteFileAsync(string publicId)
        {
            await _fileService.DeleteFileAsync(publicId);
        }
    }
}
