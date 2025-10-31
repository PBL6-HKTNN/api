namespace Codemy.Courses.Application.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public decimal Price { get; set; }
        public string Language { get; set; }
        public string Level { get; set; }
        public decimal AverageRating { get; set; }
        public int NumberOfReviews { get; set; }
    }
}
