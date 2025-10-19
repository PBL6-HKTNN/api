using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Codemy.FileStorage.Application.Interfaces;
using Codemy.FileStorage.Application.DTOs;
using Codemy.FileStorage.Infrastructure.Configurations;

namespace Codemy.FileStorage.Infrastructure.Cloudinary
{
    public class CloudinaryService : IFileService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new CloudinaryDotNet.Cloudinary(acc);
        }

        public async Task<FileUploadResult> UploadImageAsync(IFormFile file)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "codemy/images"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return new FileUploadResult
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId,
                Type = "image"
            };
        }

        public async Task<FileUploadResult> UploadVideoAsync(IFormFile file)
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "codemy/videos"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return new FileUploadResult
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId,
                Type = "video"
            };
        }

        public async Task<FileUploadResult> UploadDocumentAsync(IFormFile file)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "codemy/documents"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return new FileUploadResult
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId,
                Type = "document"
            };
        }

        public async Task DeleteFileAsync(string publicId)
        {
            await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        }
    }
}
