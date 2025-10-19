using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.Authentication
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
