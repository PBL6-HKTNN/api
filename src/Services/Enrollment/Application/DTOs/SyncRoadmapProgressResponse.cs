namespace Codemy.Enrollment.Application.DTOs
{
    public class SyncRoadmapProgressResponse
    {
        public decimal Progress { get; set; }
        public bool IsCompleted { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
