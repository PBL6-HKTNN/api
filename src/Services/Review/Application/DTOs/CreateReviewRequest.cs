using System.ComponentModel.DataAnnotations;

namespace Codemy.Review.Application.Requests
{
    public class CreateReviewRequest
    {
        [Required]
        public Guid CourseId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}
