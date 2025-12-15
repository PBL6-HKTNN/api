using Codemy.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.User
{
    public class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required Role Role { get; set; }
    }
}
