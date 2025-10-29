namespace Codemy.Search.Application.DTOs
{
    public class CourseSearchResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal Price { get; set; }
        public string Level { get; set; } = null!;
        public string Language { get; set; } = null!;
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        // Optional fields để frontend hiển thị đẹp hơn
        public string? Thumbnail { get; set; }
        public string? InstructorName { get; set; }
    }
}
