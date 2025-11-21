namespace Codemy.Courses.Application.DTOs
{
    public class GetCoursesResponse
    {
        public Guid Id { get; set;}
        public Guid instructorId { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? thumbnail { get; set; }
        public int status { get; set; }
        public TimeSpan duration { get; set; }
        public decimal price { get; set; }
        public int level { get; set; }
        public int numberOfModules { get; set; }
        public Guid categoryId { get; set; }
        public string? language { get; set; }
        public int numberOfReviews { get; set; }
        public decimal averageRating { get; set; }
        public bool IsEnrolled { get; set; }
    }
}