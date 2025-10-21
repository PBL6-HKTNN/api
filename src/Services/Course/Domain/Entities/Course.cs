using Codemy.BuildingBlocks.Domain; 
using Codemy.Courses.Domain.Enums;

namespace Codemy.Courses.Domain.Entities
{
    public class Course : BaseEntity
    {
        public Guid instructorId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
        public Status status { get; set; }
        public TimeSpan duration { get; set; }
        public decimal price { get; set; }
        public Level level { get; set; }
        public Guid categoryId { get; set; }
        public string language { get; set; }
        public int numberOfReviews { get; set; }
        public decimal averageRating { get; set; }
    }
}
