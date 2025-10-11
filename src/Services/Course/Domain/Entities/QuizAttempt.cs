using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;
using Codemy.Course.Domain.Enum;

namespace Codemy.Course.Domain.Entities
{
    internal class QuizAttempt : BaseEntity
    {
        public long userId { get; set; }
        public long quizId { get; set; } 
        public int score { get; set; } 
        public DateTime attemptedAt { get; set; }
        public DateTime completedAt { get; set; }
        public QuizAttemptStatus status { get; set; }
    }
}
