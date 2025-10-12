using Codemy.BuildingBlocks.Domain;

namespace Codemy.Course.Domain.Entities
{
    internal class Answer : BaseEntity
    {
        public Guid questionId { get; set; }
        public string answerText { get; set; }
        public bool isCorrect { get; set; }
    }
}
