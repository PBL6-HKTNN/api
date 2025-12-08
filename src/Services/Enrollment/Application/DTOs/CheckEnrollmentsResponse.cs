namespace Codemy.Enrollment.Application.DTOs
{
    public class CheckEnrollmentsResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } = string.Empty;
        public List<string> EnrolledCourseIds { get; set; } = new();
    }
}