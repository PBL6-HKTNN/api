using Codemy.BuildingBlocks.Domain;
using Codemy.Courses.Domain.Enums;

namespace Codemy.Courses.Domain.Entities
{
    public class Lesson : BaseEntity
    { 
        public string title { get; set; }
        public string contentUrl { get; set; }
        public TimeSpan duration { get; set; }
        public int orderIndex { get; set; }
        public Guid moduleId { get; set; }
        public bool isPreview { get; set; }
        public LessonType lessonType { get; set; }
    }
}
