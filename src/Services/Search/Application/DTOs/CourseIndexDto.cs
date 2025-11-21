namespace Codemy.Search.Application.DTOs
{
    public class CourseIndexDto
    {
        public Guid Id { get; set; }
        public Guid InstructorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public int Status { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }
        public int Level { get; set; }
        public int NumberOfModules { get; set; }
        public Guid CategoryId { get; set; }
        public string Language { get; set; }
        public int NumberOfReviews { get; set; }
        public decimal AverageRating { get; set; }
    }
}
