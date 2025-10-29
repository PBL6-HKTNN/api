namespace Codemy.Enrollment.Application.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public decimal Price { get; set; }
        public Guid InstructorId { get; set; }
    }
}
