using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.API.DTOs 
{
    public class ResponseResetPassword
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Token { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string NewPassword { get; set; }
    }
}
