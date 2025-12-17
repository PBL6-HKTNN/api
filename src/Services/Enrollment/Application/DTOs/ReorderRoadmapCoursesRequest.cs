namespace Codemy.Enrollment.Application.DTOs
{
    public class ReorderRoadmapCoursesRequest
    {
        public List<Guid> CourseIds { get; set; } = new();
    }
}