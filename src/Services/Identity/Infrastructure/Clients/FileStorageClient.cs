using Codemy.Identity.Application.DTOs.User;
using Codemy.Identity.Application.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Codemy.Identity.Infrastructure.Clients
{
    public class FileStorageClient : IFileStorageClient
    {
        private readonly HttpClient _httpClient;

        public FileStorageClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FileUploadResult?> UploadImageAsync(Stream fileStream, string fileName, string contentType)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is empty", nameof(fileStream));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required", nameof(fileName));

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync("/api/files/image", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<FileUploadResult>();
        }
    }
}
