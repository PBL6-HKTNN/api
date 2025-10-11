using Codemy.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Course.Domain.Entities
{
    internal class UserAnswer : BaseEntity
    {
        public long attemptId { get; set; }
        public long questionId { get; set; }
        public long answerId { get; set; }
        public int marksObtained { get; set; }
    }
}
