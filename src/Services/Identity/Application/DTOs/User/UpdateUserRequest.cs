using Codemy.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.User
{
    public class UpdateUserRequest
    {
        [Required]
        public required Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
        public Role? Role { get; set; }
    }
}
