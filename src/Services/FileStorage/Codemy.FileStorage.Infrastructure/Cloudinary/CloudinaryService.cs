using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Codemy.FileStorage.Application.Interfaces;
using Codemy.FileStorage.Application.DTOs;

namespace Codemy.FileStorage.Infrastructure.Cloudinary
{
    public class CloudinaryService : IFileService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(CloudinaryDotNet.Cloudinary cloudinary)
        {
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        } 

        //public async Task<double> GetVideoDurationFromCloudinary(string publicId)
        //{
        //    var account = new Account(
        //        "your_cloud_name",
        //        "your_api_key",
        //        "your_api_secret"
        //    );

        //    var cloudinary = new Cloudinary(account);
        //    var result = await cloudinary.GetResourceAsync(new GetResourceParams(publicId)
        //    {
        //        ResourceType = ResourceType.Video
        //    });

        //    return result.Duration; // tính bằng giây
        //}

    public async Task<FileUploadResponse> UploadImageAsync(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "File cannot be null");

            Console.WriteLine($"[CloudinaryService] Uploading image: {file.FileName}, Type: {file.ContentType}");

            // Determine the actual file type — if not an image, use RawUploadParams
            var isImage = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

            var uploadParams = isImage
                ? new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "codemy/images"
                }
                : new RawUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "codemy/files"
                };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult == null)
                throw new Exception("Upload failed: Cloudinary returned null result");

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");

            return new FileUploadResponse
            {
                Url = uploadResult.SecureUrl?.AbsoluteUri ?? string.Empty,
                PublicId = uploadResult.PublicId,
                Type = isImage ? "image" : "file"
            };
        }

        public async Task<FileUploadResponse> UploadVideoAsync(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            Console.WriteLine($"[CloudinaryService] Uploading video: {file.FileName}");

            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "codemy/videos"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult == null)
                throw new Exception("Upload failed: Cloudinary returned null result");

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");

            return new FileUploadResponse
            {
                Url = uploadResult.SecureUrl?.AbsoluteUri ?? string.Empty,
                PublicId = uploadResult.PublicId,
                Type = "video"
            };
        }

        public async Task<FileUploadResponse> UploadDocumentAsync(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            Console.WriteLine($"[CloudinaryService] Uploading document: {file.FileName}");

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "codemy/documents"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult == null)
                throw new Exception("Upload failed: Cloudinary returned null result");

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");

            return new FileUploadResponse
            {
                Url = uploadResult.SecureUrl?.AbsoluteUri ?? string.Empty,
                PublicId = uploadResult.PublicId,
                Type = "document"
            };
        }

        public async Task DeleteFileAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentNullException(nameof(publicId), "PublicId cannot be null or empty");

            Console.WriteLine($"[CloudinaryService] Deleting file: {publicId}");
            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            if (result.Error != null)
                throw new Exception($"Cloudinary delete error: {result.Error.Message}");
        }
    }
}
