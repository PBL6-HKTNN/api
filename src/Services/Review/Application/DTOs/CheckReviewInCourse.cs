using System.ComponentModel.DataAnnotations;

namespace Codemy.Review.Application.DTOs
{
    public class CheckReviewInCourse
    {
        [Required]
        public required Guid CourseId { get; set; }
        [Required]
        public required Guid ReviewId { get; set; }
    }
}
