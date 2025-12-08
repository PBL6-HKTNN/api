using System.ComponentModel.DataAnnotations;

namespace Codemy.Enrollment.Application.DTOs
{
    public class UpdateCurrentViewRequest
    {
        [Required]
        public required Guid CourseId { get; set; }
        [Required]
        public required Guid CurrentLessonId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "WatchedSeconds must be non-negative")]
        public int? WatchedSeconds { get; set; }
    }
}
