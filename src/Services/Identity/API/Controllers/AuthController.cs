using Codemy.BuildingBlocks.Core;
using Codemy.Identity.API.DTOs; 
using Codemy.Identity.Application.DTOs.Authentication;
using Codemy.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc; 

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthenticationService authenticationService, ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authenticationService.LoginAsync(request);

                if (!result.Success)
                {
                    return this.UnauthorizedResponse(result.Message ?? "Login failed");
                }

                var response = new LoginResponse
                {
                    Token = result.Token!,
                    User = result.User!
                };

                return this.OkResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing register request");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during register",
                    ex.Message
                );
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Register request)
        { 
            try
            {
                var result = await _authenticationService.CreateAccountAsync(request);

                if (!result.Success)
                {
                    return this.UnauthorizedResponse(result.Message ?? "Create account failed");
                }

                var response = new LoginResponse
                {
                    Token = result.Token!,
                    User = result.User!
                };

                return this.OkResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing register request");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during register",
                    ex.Message
                );
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    await _authenticationService.RevokeTokenAsync(userId);
                }

                return this.OkResponse(new LogoutResponse { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return this.InternalServerErrorResponse(
                    "Error occurred during logout",
                    ex.Message
                );
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }

            try
            {
                var result = await _authenticationService.AuthenticateWithGoogleAsync(request.Token);

                if (!result.Success)
                {
                    return this.UnauthorizedResponse(result.Message ?? "Authentication failed");
                }
                 
                var response = new LoginResponse
                {
                    Token = result.Token!,
                    User = result.User!
                };

                return this.OkResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Google login request");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during authentication",
                    ex.Message
                );
            }
        }
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyToken token)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }

            try
            {
                var result = await _authenticationService.verifyEmail(token.Email,token.Token);

                if (!result.Success)
                {
                    return this.UnauthorizedResponse(result.Message ?? "Verify email failed");
                }

                var response = new LoginResponse
                {
                    Token = result.Token!,
                    User = result.User!
                };

                return this.OkResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verify email");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during authentication",
                    ex.Message
                );
            }
        }
    }
}
