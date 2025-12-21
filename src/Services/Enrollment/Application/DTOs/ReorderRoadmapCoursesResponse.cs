namespace Codemy.Enrollment.Application.DTOs
{
    public class ReorderRoadmapCoursesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public Guid RoadmapId { get; set; }
    }

}