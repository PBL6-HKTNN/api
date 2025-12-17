namespace Codemy.Enrollment.Application.DTOs
{
    public class RemoveCourseFromRoadmapResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public Guid RoadmapId { get; set; }
        public Guid CourseId { get; set; }
    }

}