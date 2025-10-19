using Codemy.Identity.Application.DTOs.Authentication;
using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateWithGoogleAsync(string googleToken);
        string GenerateJwtTokenAsync(User user);
        bool ValidateJwtTokenAsync(string token); 
        Task<User?> GetUserFromJwtAsync(string token);
        Task<AuthenticationResult> CreateAccountAsync(Register request);
        Task<AuthenticationResult> LoginAsync(LoginRequest request);
        Task RevokeTokenAsync(Guid userId);
        Task<AuthenticationResult> verifyEmail(string Email, string token);
        Task<SendResetPasswordResult> GetResetPasswordToken(string email);
        Task<SendResetPasswordResult> ResetPassword(string email, string token, string newPassword);
        Task<SendResetPasswordResult> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);

    }

    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public User? User { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class SendResetPasswordResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class GoogleUserInfo
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; } 
        public string? Picture { get; set; }
        public bool EmailVerified { get; set; } 
    }
}
