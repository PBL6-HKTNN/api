using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;
namespace Codemy.Course.Domain.Entities
{
    internal class Quiz : BaseEntity
    {
        public long lessonId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int totalMarks { get; set; }
        public int passingMarks { get; set; }
    }
}
