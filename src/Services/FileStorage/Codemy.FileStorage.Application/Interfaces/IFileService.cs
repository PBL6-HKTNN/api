using Microsoft.AspNetCore.Http;
using Codemy.FileStorage.Application.DTOs;

namespace Codemy.FileStorage.Application.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadImageAsync(IFormFile file);
        Task<FileUploadResult> UploadVideoAsync(IFormFile file);
        Task<FileUploadResult> UploadDocumentAsync(IFormFile file);
        Task DeleteFileAsync(string publicId);
    }
}
