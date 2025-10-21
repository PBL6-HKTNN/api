using System.ComponentModel.DataAnnotations;

namespace Codemy.Enrollment.API.DTOs
{
    public class AddToWishlistRequest
    {
        [Required]
        public required Guid UserId { get; set; }
        [Required]
        public required Guid CourseId { get; set; }
    }
}
