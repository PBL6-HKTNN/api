using System.ComponentModel.DataAnnotations;

namespace Codemy.Enrollment.Application.DTOs
{
    public class UpdateCurrentViewRequest
    {
        [Required]
        public required Guid CourseId { get; set; }
        [Required]
        public required Guid CurrentLessonId { get; set; }
    }
}
