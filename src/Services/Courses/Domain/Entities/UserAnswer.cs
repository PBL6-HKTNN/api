using Codemy.BuildingBlocks.Domain;

namespace Codemy.Courses.Domain.Entities
{
    internal class UserAnswer : BaseEntity
    {
        public Guid attemptId { get; set; }
        public Guid questionId { get; set; }
        public Guid answerId { get; set; }
        public int marksObtained { get; set; }
    }
}
