using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.API.DTOs
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required User User { get; set; }
    }
}
