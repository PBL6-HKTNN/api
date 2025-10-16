using Microsoft.AspNetCore.Mvc;
using Codemy.Notification.Application.Interfaces;
using Codemy.Notification.Application.DTOs;
using Codemy.BuildingBlocks.Core;

namespace Codemy.Notification.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailContent content)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _emailService.SendEmailAsync(content.From, content.To, content.Token); 
            return Ok();
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] EmailContent content)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _emailService.SendResetPasswordToken(content.From, content.To, content.Token);
            return Ok();
        }
    }
}
