using Codemy.BuildingBlocks.Core;
using Codemy.Identity.Application.DTOs.Authentication;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Codemy.Identity.Application.Services
{
    internal class AuthenticationService : IAuthenticationService 
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IRepository<User> _userRepository; 
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly int _jwtExpirationHours;  

        public AuthenticationService(
            IConfiguration configuration,
            ILogger<AuthenticationService> logger,
            IRepository<User> userRepository, 
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _logger = logger;
            _userRepository = userRepository; 
            _unitOfWork = unitOfWork;

            _jwtSecret = _configuration["Jwt:Secret"] ?? throw new ArgumentException("JWT Secret not configured");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentException("JWT Issuer not configured");
            _jwtExpirationHours = _configuration.GetValue<int>("Jwt:ExpirationHours"); 
        }
        public async Task<AuthenticationResult> AuthenticateWithGoogleAsync(string googleToken)
        {
            try
            {
                _logger.LogInformation("Starting Google authentication process");

                // Validate Google token
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_configuration["Authentication:Google:ClientId"]]
                });

                _logger.LogInformation("Google token validated successfully for user: {Email}", payload.Email);

                // Get or create user
                var user = await GetOrCreateUserAsync(new GoogleUserInfo
                {
                    Id = payload.Subject,
                    Email = payload.Email,
                    Name = payload.Name, 
                    Picture = payload.Picture,
                    EmailVerified = payload.EmailVerified, 
                });

                if (user == null)
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = "Failed to create or retrieve user account"
                    };
                } 
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate JWT token
                var jwtToken = GenerateJwtTokenAsync(user);

                _logger.LogInformation("Authentication successful for user: {Email}", user.email);

                return new AuthenticationResult
                {
                    Success = true,
                    Token = jwtToken,
                    User = user,
                    Message = "Authentication successful"
                };
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning("Invalid Google token: {Error}", ex.Message);
                return new AuthenticationResult
                {
                    Success = false,
                    Message = "Invalid Google token"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google authentication");
                return new AuthenticationResult
                {
                    Success = false,
                    Message = "Authentication failed. Please try again."
                };
            }
        }

        private async Task<User> GetOrCreateUserAsync(GoogleUserInfo googleUserInfo)
        { 
            var usersByGoogleId = await _userRepository.FindAsync(u => u.googleId == googleUserInfo.Id);
            var user = usersByGoogleId.FirstOrDefault();

            if (user == null)
            { 
                var usersByEmail = await _userRepository.FindAsync(u => u.email == googleUserInfo.Email);
                user = usersByEmail.FirstOrDefault();

                if (user != null)
                { 
                    user.googleId = googleUserInfo.Id;
                    user.profilePicture = googleUserInfo.Picture;
                    _userRepository.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    return user;
                }

                user = new User
                {
                    email = googleUserInfo.Email,
                    name = googleUserInfo.Name ?? googleUserInfo.Email.Split('@')[0],
                    googleId = googleUserInfo.Id,
                    profilePicture = googleUserInfo.Picture ?? "https://example.com/default-avatar.png",
                    role = Role.Student,
                    status = UserStatus.Active,
                    emailVerified = googleUserInfo.EmailVerified,
                    CreatedAt = DateTime.UtcNow, 
                    totalCourses = 0
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Created new user account for: {Email}", user.email);
            }
            else
            { 
                user.profilePicture = googleUserInfo.Picture; 
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }

            return user;
        }

        public string GenerateJwtTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.email),
                new(ClaimTypes.Name, $"{user.name}"),  
                new(ClaimTypes.Role, user.role.ToString()),
                new("google_id", user.googleId), 
            };

            if (!string.IsNullOrEmpty(user.profilePicture))
            {
                claims.Add(new Claim("Picture", user.profilePicture));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtExpirationHours),
                Issuer = _jwtIssuer,
                Audience = _jwtIssuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task<User?> GetUserFromJwtAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task RevokeTokenAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public bool ValidateJwtTokenAsync(string token)
        {
            throw new NotImplementedException();
        } 

        Task<AuthenticationResult> IAuthenticationService.CreateAccountAsync(Register request)
        {
            throw new NotImplementedException();
        }
    }
}
