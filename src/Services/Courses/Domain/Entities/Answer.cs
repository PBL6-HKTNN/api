using Codemy.BuildingBlocks.Domain;

namespace Codemy.Courses.Domain.Entities
{
    public class Answer : BaseEntity
    {
        public Guid questionId { get; set; }
        public string answerText { get; set; }
        public bool isCorrect { get; set; }
    }
}
