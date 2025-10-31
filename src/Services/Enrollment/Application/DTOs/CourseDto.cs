namespace Codemy.Enrollment.Application.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public int Status { get; set; }  
        public string? Duration { get; set; }
        public decimal Price { get; set; }
        public int Level { get; set; }
        public int NumberOfModules { get; set; }
        public Guid CategoryId { get; set; }
        public string? Language { get; set; }
        public int NumberOfReviews { get; set; }
        public double AverageRating { get; set; }
        public Guid InstructorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
