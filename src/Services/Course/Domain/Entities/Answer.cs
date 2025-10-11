using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Course.Domain.Entities
{
    internal class Answer : BaseEntity
    {
        public long questionId { get; set; }
        public string answerText { get; set; }
        public bool isCorrect { get; set; }
    }
}
