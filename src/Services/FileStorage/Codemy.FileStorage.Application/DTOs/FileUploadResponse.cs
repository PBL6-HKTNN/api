namespace Codemy.FileStorage.Application.DTOs
{
    public class FileUploadResponse
    {
        public string Url { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}
