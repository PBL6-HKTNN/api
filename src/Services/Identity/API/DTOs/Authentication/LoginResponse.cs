using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.API.DTOs
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required Domain.Entities.User User { get; set; }
    }
}
