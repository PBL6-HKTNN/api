using Codemy.Identity.Application.DTOs.User;
using Microsoft.AspNetCore.Http;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IFileStorageClient
    {
        Task<FileUploadResult?> UploadImageAsync(Stream file, string fileName, string contentType);
    }
}