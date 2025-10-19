using Codemy.FileStorage.Application.DTOs;
using System.Net.Http.Headers;

namespace Codemy.Identity.Infrastructure.HttpClients
{
    public class FileStorageClient
    {
        private readonly HttpClient _httpClient;

        public FileStorageClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FileUploadResult?> UploadImageAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            await using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("/api/file/image", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<FileUploadResult>();
        }
    }
}
