using System.ComponentModel.DataAnnotations;

namespace Codemy.Courses.Application.DTOs
{
    public class CreateQuizInLessonRequest
    {
        [Required]
        public required Guid lessonId { get; set; }
        [Required]
        public required string time { get; set; }
        [Required]
        public required string question { get; set; }
        [Required]
        public required List<string> options { get; set; }
        [Required]
        public required string correctAnswer { get; set; }
    }
}
