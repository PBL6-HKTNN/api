namespace Codemy.Search.Application.DTOs
{
    public class CourseIndexDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal Price { get; set; }
        public string Language { get; set; } = null!;
        public string Level { get; set; } = null!;
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
