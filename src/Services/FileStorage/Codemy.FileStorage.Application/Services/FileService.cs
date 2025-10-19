// using Codemy.FileStorage.Application.DTOs;
// using Codemy.FileStorage.Application.Interfaces;
// using Microsoft.AspNetCore.Http;

// namespace Codemy.FileStorage.Application.Services
// {
//     public class FileService : IFileService
//     {
//         private readonly ICloudinaryService _cloudinaryService;

//         public FileService(ICloudinaryService cloudinaryService)
//         {
//             _cloudinaryService = cloudinaryService;
//         }

//         // Upload image
//         public async Task<FileUploadResult> UploadImageAsync(IFormFile file)
//         {
//             return await UploadFileAsync(file, "image", "uploads/images");
//         }

//         // Upload video
//         public async Task<FileUploadResult> UploadVideoAsync(IFormFile file)
//         {
//             return await UploadFileAsync(file, "video", "uploads/videos");
//         }

//         // Upload document (pdf, docx, txt, v.v.)
//         public async Task<FileUploadResult> UploadDocumentAsync(IFormFile file)
//         {
//             return await UploadFileAsync(file, "document", "uploads/documents");
//         }

//         // Delete file
//         public async Task DeleteFileAsync(string publicId)
//         {
//             await _cloudinaryService.DeleteFileAsync(publicId);
//         }

//         // Hàm private dùng chung cho tất cả loại file
//         private async Task<FileUploadResult> UploadFileAsync(IFormFile file, string type, string folder)
//         {
//             if (file == null || file.Length == 0)
//                 throw new ArgumentException("File is empty or null.");

//             await using var stream = file.OpenReadStream();
//             var fileName = Path.GetFileName(file.FileName);

//             var (url, publicId) = await _cloudinaryService.UploadFileAsync(stream, fileName, folder, type);

//             return new FileUploadResult
//             {
//                 Url = url,
//                 PublicId = publicId,
//                 Type = type
//             };
//         }
//     }
// }
