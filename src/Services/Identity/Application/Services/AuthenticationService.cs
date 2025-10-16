using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Identity.Application.DTOs.Authentication;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using DotNetEnv;
using Google.Apis.Auth; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
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
        private readonly PasswordHasher<string> _hasher = new();

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

            LogExtensions.LoadEnvFile(_logger);

            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new ArgumentException("JWT Secret not configured");
            _jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentException("JWT Issuer not configured");
            _jwtExpirationHours = Int32.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS")!);
        }


        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, inputPassword);
            return result == PasswordVerificationResult.Success;
        }
        public async Task<AuthenticationResult> AuthenticateWithGoogleAsync(string googleToken)
        {
            try
            {
                _logger.LogInformation("Starting Google authentication process");

                // Validate Google token
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")]
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
                    if(!user.emailVerified) user.emailVerified = googleUserInfo.EmailVerified;
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
                    profilePicture = googleUserInfo.Picture ?? "",
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

        public async Task RevokeTokenAsync(Guid userId)
        {
            var users = await _userRepository.FindAsync(u => u.Id == userId);
            var user = users.FirstOrDefault();
            if (user != null)
            { 
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public bool ValidateJwtTokenAsync(string token)
        {
            throw new NotImplementedException();
        } 

        async Task<AuthenticationResult> IAuthenticationService.CreateAccountAsync(Register request)
        {
            var usersByEmail = await _userRepository.FindAsync(u => u.email == request.email);
            var user = usersByEmail.FirstOrDefault();
            if (user != null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Message = "Email is already registered"
                };
            }

            user = new User
            {
                email = request.email,
                name = request.email.Split('@')[0],
                googleId = "",
                profilePicture = "",
                role = Role.Student,
                status = UserStatus.Active,
                emailVerified = false,
                CreatedAt = DateTime.UtcNow,
                emailVerificationToken = Guid.NewGuid().ToString(),
                passwordHash = HashPassword(request.password),
                totalCourses = 0
            };

            //send mail to verify email

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created new user account for: {Email}", request.email);

            return new AuthenticationResult
            {
                Success = true,
                Token = GenerateJwtTokenAsync(user),
                User = user,
                Message = "Register successful"
            };
        }

        public async Task<AuthenticationResult> LoginAsync(LoginRequest request)
        {
            var usersByEmail = await _userRepository.FindAsync(u => u.email == request.email);
            var user = usersByEmail.FirstOrDefault();
            if (user != null)
            {
                if (!user.emailVerified)
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = "Your email address is not verified. Please check your inbox"
                    };
                }
                if (VerifyPassword(user.passwordHash, request.password))
                {
                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = GenerateJwtTokenAsync(user),
                        User = user,
                        Message = "Login successful"
                    };
                }
                return new AuthenticationResult
                {
                    Success = false,
                    Message = "Invalid email or password. Please try again"
                };
            }
            return new AuthenticationResult
            {
                Success = false,
                Message = "Invalid email or password. Please try again"
            };
        }

        public async Task<AuthenticationResult> verifyEmail(string Email, string token)
        {
            var usersByEmail = await _userRepository.FindAsync(u => u.email == Email);
            var user = usersByEmail.FirstOrDefault();
            if (user != null)
            {
                if (user.emailVerificationToken.Equals(token))
                {
                    user.emailVerified = true;
                    user.emailVerificationToken = null;
                    _userRepository.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = GenerateJwtTokenAsync(user),
                        User = user,
                        Message = "Verify email successful"
                    };
                }
                else
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = "Invalid token. Please try again"
                    }; 
                } 
            }
            return new AuthenticationResult
            {
                Success = false,
                Message = "Invalid email"
            };
        }
    }
}
