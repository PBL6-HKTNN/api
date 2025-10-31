namespace Codemy.Courses.Application.DTOs
{
    public class MyCourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Level { get; set; }
        public decimal Price { get; set; }
        public decimal AverageRating { get; set; }
        public string Language { get; set; }

        // enrollment
        public string Progress { get; set; }
        public string Status { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
