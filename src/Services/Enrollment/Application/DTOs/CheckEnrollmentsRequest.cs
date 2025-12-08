namespace Codemy.Enrollment.Application.DTOs
{
    public class CheckEnrollmentsRequest
    {
        public Guid UserId { get; set; }
        public List<Guid> CourseIds { get; set; } = new List<Guid>();
    }
}


