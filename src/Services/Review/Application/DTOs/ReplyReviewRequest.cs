using System.ComponentModel.DataAnnotations;

namespace Codemy.Review.Application.DTOs {
    public class ReplyReviewRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Reply { get; set; } = string.Empty;
    }
}