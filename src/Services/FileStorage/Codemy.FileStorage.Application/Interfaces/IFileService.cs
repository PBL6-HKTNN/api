using Microsoft.AspNetCore.Http;
using Codemy.FileStorage.Application.DTOs;

namespace Codemy.FileStorage.Application.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResponse> UploadImageAsync(IFormFile file);
        Task<FileUploadResponse> UploadVideoAsync(IFormFile file);
        Task<FileUploadResponse> UploadDocumentAsync(IFormFile file);
        Task DeleteFileAsync(string publicId);
    }
}
