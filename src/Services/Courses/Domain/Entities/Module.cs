using Codemy.BuildingBlocks.Domain;

namespace Codemy.Courses.Domain.Entities
{
    public class Module : BaseEntity
    {
        public Guid courseId { get; set; }
        public string title { get; set; }
        public TimeSpan duration { get; set; }
        public int numberOfLessons { get; set; }
        public int order { get; set; }
    }
}
