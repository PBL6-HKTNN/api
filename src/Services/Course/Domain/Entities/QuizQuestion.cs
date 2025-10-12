using Codemy.BuildingBlocks.Domain;
using Codemy.Course.Domain.Enums;

namespace Codemy.Course.Domain.Entities
{
    internal class QuizQuestion : BaseEntity
    {
        public Guid quizId { get; set; }
        public string questionText { get; set; }
        public QuestionType questionType { get; set; }
        public int marks { get; set; }
    }
}
