using System.Text.Json.Serialization;

namespace Codemy.Identity.Application.DTOs.User
{
    public class FileUploadResult
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("data")]
        public FileUploadData Data { get; set; } = new FileUploadData();

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }
    }

    public class FileUploadData
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("publicId")]
        public string PublicId { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}

