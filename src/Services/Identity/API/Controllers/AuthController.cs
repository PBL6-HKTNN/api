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
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Dummy authentication logic for demonstration purposes
            if (request.Username == "user" && request.Password == "password")
            { 
                return this.OkResponse("login");
            }
            return Unauthorized();
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
        public IActionResult Logout()
        {
            // Dummy logout logic for demonstration purposes
            return this.OkResponse("logout");
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
    }
}
