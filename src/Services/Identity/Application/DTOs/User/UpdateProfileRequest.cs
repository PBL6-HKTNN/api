using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.API.DTOs.User
{
    public class UpdateProfileRequest
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; } = default!;
        [Required]
        [MaxLength(300)]
        public required string Bio { get; set; } = default!;
    }
}
