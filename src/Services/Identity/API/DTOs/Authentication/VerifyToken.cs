using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.API.DTOs
{
    public class VerifyToken
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Token { get; set; }
    }
}
