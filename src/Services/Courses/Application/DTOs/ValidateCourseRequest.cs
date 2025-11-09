using System.ComponentModel.DataAnnotations;

namespace Codemy.Courses.Application.DTOs
{
    public class ValidateCourseRequest
    {
        [Required]
        public required Guid CourseId { get; set; }
        [Required]
        public required Guid LessonId { get; set; }
    }
}
