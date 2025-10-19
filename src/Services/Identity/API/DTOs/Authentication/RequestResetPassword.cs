using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.API.DTOs
{
    public class RequestResetPassword
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; } 
    }
}
