using Codemy.BuildingBlocks.Domain;
using Codemy.Course.Domain.Enums;

namespace Codemy.Course.Domain.Entities
{
    internal class Lesson : BaseEntity
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
