using Codemy.BuildingBlocks.Domain;
using Codemy.Courses.Domain.Enums;

namespace Codemy.Courses.Domain.Entities
{
    public class QuizAttempt : BaseEntity
    {
        public Guid userId { get; set; }
        public Guid quizId { get; set; } 
        public int score { get; set; } 
        public DateTime attemptedAt { get; set; }
        public DateTime? completedAt { get; set; }
        public QuizAttemptStatus status { get; set; }
    }
}
