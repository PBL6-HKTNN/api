using Codemy.BuildingBlocks.Domain;
using Codemy.Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Course.Domain.Entities
{
    internal class QuizQuestion : BaseEntity
    {
        public long quizId { get; set; }
        public string questionText { get; set; }
        public QuestionType questionType { get; set; }
        public int marks { get; set; }
    }
}
