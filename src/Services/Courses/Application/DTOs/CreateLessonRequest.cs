using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.DTOs
{
    public class CreateLessonRequest
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string contentUrl { get; set; } 
        [Required]
        public int orderIndex { get; set; }
        [Required]
        public Guid moduleId { get; set; }
        [Required]
        public bool isPreview { get; set; }
        [Required]
        public int lessonType { get; set; }
        [Required]
        public double duration { get; set; }

    }
}
