using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.API.DTOs
{
    public class GoogleLoginRequest
    {
        [Required]
        public required string Token { get; set; }
    }
}
