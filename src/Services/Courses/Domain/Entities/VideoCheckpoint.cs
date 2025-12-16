using Codemy.BuildingBlocks.Domain;

namespace Codemy.Courses.Domain.Entities
{
    public class VideoCheckpoint : BaseEntity
    {
        public Guid LessonId { get; set; }
        public string Time { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
