namespace Codemy.FileStorage.Application.DTOs
{
    public class FileUploadResult
    {
        public string Url { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}
