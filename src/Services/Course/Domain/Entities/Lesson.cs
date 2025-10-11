using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Course.Domain.Entities
{
    internal class Lesson : BaseEntity
    { 
        public string title { get; set; }
        public string contentUrl { get; set; }
        public TimeSpan duration { get; set; }
        public int orderIndex { get; set; }
        public long moduleId { get; set; }
        public bool isPreview { get; set; }
        public LessonType lessonType { get; set; }
    }
}
