using Codemy.FileStorage.Application.Services;
using Codemy.FilesProto;
using Grpc.Core;
using CloudinaryDotNet;
using Codemy.FileStorage.Infrastructure.Configurations;

namespace Codemy.FileStorage.API.Services
{
    public class FilesGrpcService : FileUploader.FileUploaderBase
    {
        private readonly FileStorageAppService _fileService;
        private readonly Cloudinary _cloudinary;

        public FilesGrpcService(FileStorageAppService fileService, Cloudinary cloudinary)
        {
            _fileService = fileService;
            _cloudinary = cloudinary;
        }

        public override async Task<UploadFileResponse> UploadFile(
            IAsyncStreamReader<UploadFileRequest> requestStream,
            ServerCallContext context)
        {
            string? fileName = null;
            string? type = null;

            var ms = new MemoryStream();

            await foreach (var chunk in requestStream.ReadAllAsync(context.CancellationToken))
            {
                if (fileName == null)
                {
                    if (string.IsNullOrWhiteSpace(chunk.FileName) ||
                        string.IsNullOrWhiteSpace(chunk.Type))
                    {
                        throw new RpcException(new Status(
                            StatusCode.InvalidArgument,
                            "file_name and type must be provided in the first chunk"
                        ));
                    }

                    fileName = chunk.FileName;
                    type = chunk.Type;
                }

                if (chunk.ChunkData?.Length > 0)
                {
                    await ms.WriteAsync(chunk.ChunkData.Memory);
                }
            }

            ms.Position = 0;

            var formFile = new FormFile(
                ms,
                0,
                ms.Length,
                "file",
                fileName!
            );

            try
            {
                var result = await _fileService.UploadFileAsync(formFile, type!);

                return new UploadFileResponse
                {
                    FileUrl = result.Url,
                    PublicId = result.PublicId,
                    Message = "Uploaded successfully"
                };
            }
            finally
            {
                await ms.DisposeAsync();
            }
        }

        public override Task<DownloadFileResponse> DownloadFile(
            DownloadFileRequest request,
            ServerCallContext context)
        {
            string publicId = request.PublicId.Trim();
            if (publicId.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                publicId = publicId.Substring(0, publicId.Length - 4);
            }

            string version = request.Version;
            if (!string.IsNullOrEmpty(version) && version.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                version = version.Substring(1);
            }

            var urlBuilder = _cloudinary.Api.UrlImgUp
                .Transform(new Transformation().Flags("attachment")) 
                .Format("pdf"); 

            if (!string.IsNullOrEmpty(version))
            {
                urlBuilder.Version(version);
            }

            string publicUrl = urlBuilder.BuildUrl(publicId);

            return Task.FromResult(new DownloadFileResponse
            {
                DownloadUrl = publicUrl
            });
        }
    }
}
