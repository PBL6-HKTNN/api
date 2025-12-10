using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.Authentication
{
    public class GoogleCodeRequest
    {
        [Required]
        public required string Code { get; set; }
    }
}
