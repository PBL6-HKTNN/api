namespace Codemy.Enrollment.Application.DTOs
{
    public class UpdateRoadmapResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public Guid RoadmapId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

}