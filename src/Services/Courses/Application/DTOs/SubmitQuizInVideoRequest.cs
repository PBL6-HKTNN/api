using System.ComponentModel.DataAnnotations;

namespace Codemy.Courses.Application.DTOs
{
    public class SubmitQuizInVideoRequest
    {
        [Required]
        public required Guid VideoCheckpointId { get; set; }
        [Required]
        public required string Answer { get; set; }
    }
}
