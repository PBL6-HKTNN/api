using System.ComponentModel.DataAnnotations;

namespace Codemy.Enrollment.Application.DTOs
{
    public class UpdateProgressRequest
    {
        [Required]
        public required Guid CourseId { get; set; }
        [Required]
        public required Guid LessonId { get; set; }
    }
}
