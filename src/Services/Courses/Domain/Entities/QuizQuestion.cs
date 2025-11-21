using Codemy.BuildingBlocks.Domain;
using Codemy.Courses.Domain.Enums;

namespace Codemy.Courses.Domain.Entities
{
    public class QuizQuestion : BaseEntity
    {
        public Guid quizId { get; set; }
        public string questionText { get; set; }
        public QuestionType questionType { get; set; }
        public int marks { get; set; }
    }
}
