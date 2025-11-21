using Codemy.BuildingBlocks.Domain;

namespace Codemy.Courses.Domain.Entities
{
    public class UserAnswer : BaseEntity
    {
        public Guid attemptId { get; set; }
        public Guid questionId { get; set; }
        public string? answerText { get; set; }
        public Guid? answerId { get; set; }
    }
}
