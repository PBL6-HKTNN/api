using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Identity.Application.DTOs.Authentication;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Action = Codemy.Identity.Domain.Entities.Action;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace Codemy.Identity.Application.Services
{
    internal class AuthenticationService : IAuthenticationService 
    { 
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPermissionGroup> _userPermissionRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<PermissionGroup> _permissionGroupRepository;
        private readonly IRepository<Action> _actionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly int _jwtExpirationHours;
        private readonly PasswordHasher<string> _hasher = new();
        private readonly EmailSender _emailSender;
        private readonly string _email;

        public AuthenticationService( 
            ILogger<AuthenticationService> logger,
            IRepository<User> userRepository, 
            IRepository<UserPermissionGroup> userPermissionRepository,
            IRepository<Permission> permissionRepository,
            IRepository<PermissionGroup> permissionGroupRepository,
            IRepository<Action> actionRepository,
            IUnitOfWork unitOfWork,
            EmailSender emailSender)
        { 
            _logger = logger;
            _userRepository = userRepository; 
            _userPermissionRepository = userPermissionRepository;
            _permissionRepository = permissionRepository;
            _permissionGroupRepository = permissionGroupRepository;
            _actionRepository = actionRepository;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;

            LogExtensions.LoadEnvFile(_logger);

            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new ArgumentException("JWT Secret not configured");
            _jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentException("JWT Issuer not configured");
            _email = Environment.GetEnvironmentVariable("SMTP_USER") ?? throw new ArgumentException("Email not configured");
            _jwtExpirationHours = Int32.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS")!);
        }

        public string HashPassword(string password)
        {
            return _hasher.HashPassword("", password);
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            var result = _hasher.VerifyHashedPassword("", hashedPassword, inputPassword);
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
                var jwtToken = await GenerateJwtTokenAsync(user);

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
                    user.totalLoginFailures = 0;
                    user.resetPasswordToken = null;
                    user.resetPasswordTokenExpiry = null; 
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
                user.resetPasswordToken = null;
                user.resetPasswordTokenExpiry = null;
                user.totalLoginFailures = 0; 
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }

            return user;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            // 1. Get permissions from user
            var userPermissions = await _userPermissionRepository
                .FindAsync(p => p.UserId == user.Id);

            // 2. Get permissions from role
            var rolePermissions = await _userPermissionRepository
                .FindAsync(p => p.RoleId == user.role);

            // 3. Merge + remove duplicates by PermissionId
            var allPermissionGroups = userPermissions
                .Concat(rolePermissions)
                .GroupBy(p => p.PermissionId)
                .Select(g => g.First())
                .ToList();

            List<Guid> permissionIds = allPermissionGroups.Where(g => !g.IsDeleted).Select(g => g.PermissionId).ToList();

            List<Action> actions = new List<Action>();
            //qua bảng permissionGroup lấy action rồi qua action lấy name bỏ vào token
            foreach (var permission in permissionIds)
            {
                var permissions = await _permissionGroupRepository.FindAsync(p => p.permissionId == permission);
                foreach (var permissionItem in permissions)
                {
                    Guid actionId = permissionItem.actionId;
                    Action action = await _actionRepository.GetByIdAsync(actionId);
                    actions.Add(action);
                }
            }
            List<string> actionNames = actions.Select(a => a.Name).Distinct().ToList();
            Console.WriteLine("User Actions: " + string.Join(", ", actionNames));
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.email),
                new(ClaimTypes.Name, $"{user.name}"),  
                new(ClaimTypes.Role, user.role.ToString()),
                new("google_id", user.googleId),
                new("permissions", string.Join(",", actionNames))
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
            await _emailSender.SendAsync(_email, user.email, user.emailVerificationToken);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created new user account for: {Email}", request.email);

            return new AuthenticationResult
            {
                Success = true,
                Token = await GenerateJwtTokenAsync(user),
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
                if(user.totalLoginFailures >= 5)
                {
                    if (user.resetPasswordTokenExpiry == null || user.resetPasswordTokenExpiry < DateTime.UtcNow)
                    {
                        var sendToken = await GetResetPasswordToken(user.email);
                        if (!sendToken.Success)
                        {
                            _logger.LogError("Failed to send reset password token to locked user {Email}", user.email);
                        }
                        else {
                            _logger.LogInformation("Sent reset password token to locked user {Email}", user.email);
                        }
                    } else {
                        _logger.LogInformation("User {Email} account is locked. Reset password token already sent.", user.email);
                    }
                    return new AuthenticationResult
                    {
                        Success = false,
                        Message = "Your account is locked due to multiple failed login attempts. Please check your email to reset password."
                    };
                }
                if (user.passwordHash != null && VerifyPassword(user.passwordHash, request.password))
                {
                    if (!user.emailVerified)
                    {
                        return new AuthenticationResult
                        {
                            Success = false,
                            Message = "Your email address is not verified. Please check your inbox",
                            User = user
                        };
                    }
                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = await GenerateJwtTokenAsync(user),
                        User = user,
                        Message = "Login successful"
                    };
                }
                user.totalLoginFailures++;
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
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
                if (user.emailVerificationToken != null && user.emailVerificationToken.Equals(token))
                {
                    user.emailVerified = true;
                    user.emailVerificationToken = null;
                    _userRepository.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = await GenerateJwtTokenAsync(user),
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

        public async Task<SendResetPasswordResult> GetResetPasswordToken(string email )
        {
            var usersByEmail = await _userRepository.FindAsync(u => u.email == email);
            var user = usersByEmail.FirstOrDefault();
            if (user == null)
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "Email not found. Please check and try again."
                };
            }
            var token = Guid.NewGuid().ToString("N").Substring(0, 8);
            user.resetPasswordToken = token;
            user.resetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            try
            {
                await _emailSender.SendResetPasswordToken(_email, email, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reset password email to {Email}", email);
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "Failed to send reset password email. Please try again later."
                };
            }
            return new SendResetPasswordResult
            {
                Success = true,
                Message = "Reset password email sent successfully. Please check your inbox."
            };
        }

        public async Task<SendResetPasswordResult> ResetPassword(string email, string token, string newPassword)
        {
            var usersByEmail = await _userRepository.FindAsync(u => u.email == email);
            var user = usersByEmail.FirstOrDefault();
            if (user == null)
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "Email not found. Please check and try again."
                };
            }
            if (user.resetPasswordToken != token || user.resetPasswordTokenExpiry == null || user.resetPasswordTokenExpiry < DateTime.UtcNow)
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "Invalid or expired token. Please request a new password reset."
                };
            }
            if (user.emailVerified == false)
            {
                user.emailVerified = true;
                user.emailVerificationToken = null;
            }
            user.passwordHash = HashPassword(newPassword);
            user.resetPasswordToken = null;
            user.resetPasswordTokenExpiry = null;
            user.totalLoginFailures = 0;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return new SendResetPasswordResult
            {
                Success = true,
                Message = "Password reset successful. You can now log in with your new password."
            };
        }

        public async Task<SendResetPasswordResult> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "Password fields cannot be empty."
                };
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "New password and confirm password do not match."
                };
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            if (string.IsNullOrEmpty(user.passwordHash))
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "This account doesn't have a password. Please set one via reset password."
                };
            }

            var isCurrentPasswordValid = VerifyPassword(user.passwordHash, request.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                return new SendResetPasswordResult
                {
                    Success = false,
                    Message = "Current password is incorrect."
                };
            }

            user.passwordHash = HashPassword(request.NewPassword);

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user {Email}", user.email);

            return new SendResetPasswordResult
            {
                Success = true,
                Message = "Password changed successfully."
            };
        }

        public Task<User?> GetUserById(string userId)
        {
            if (!Guid.TryParse(userId, out var guid))
            {
                _logger.LogWarning("Invalid GUID format for userId: {UserId}", userId);
                Console.WriteLine($"Invalid GUID format for userId: {userId}");
                return null;
            }
            return _userRepository.GetByIdAsync(Guid.Parse(userId));
        }
    }
}
