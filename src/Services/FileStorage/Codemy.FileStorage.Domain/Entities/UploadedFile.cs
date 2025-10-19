namespace Codemy.FileStorage.Domain.Entities
{
    public class UploadedFile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Url { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
