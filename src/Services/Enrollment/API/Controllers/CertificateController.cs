using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Codemy.Enrollment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly ILogger<CertificateController> _logger;
        public CertificateController(ICertificateService certificateService, ILogger<CertificateController> logger)
        {
            _certificateService = certificateService;
            _logger = logger;
        }

        [HttpPost("{enrollmentId}/generate")]
        public async Task<IActionResult> GenerateCertificateAsync(Guid enrollmentId)
        {
            try
            {
                var user = HttpContext.User;

                var result = await _certificateService.GenerateCertificateAsync(enrollmentId, user);

                return this.OkResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating certificate for enrollment {EnrollmentId}", enrollmentId);
                return this.BadRequestResponse(ex.Message);
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyCertificatesAsync()
        {
            try
            {
                var user = HttpContext.User;

                var result = await _certificateService.GetMyCertificatesAsync(user);

                return this.OkResponse(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access: {Message}", ex.Message);
                return this.UnauthorizedResponse("User ID not found in claims.");
            }
        }

        [HttpGet("{enrollmentId}/status")]
        public async Task<IActionResult> CheckCertificateStatusAsync(Guid enrollmentId)
        {
            var user = HttpContext.User;

            var result = await _certificateService.CheckCertificateStatusAsync(enrollmentId, user);
            if (result.Success)
            {
                return this.OkResponse(result);
            }
            else
            {
                return this.BadRequestResponse(result.Message ?? "Failed to check certificate status.");
            }
        }

        [HttpGet("{enrollmentId}/download")]
        public async Task<IActionResult> DownloadCertificateAsync(Guid enrollmentId)
        {
            try
            {
                var user = HttpContext.User;

                var result = await _certificateService.DownloadCertificateAsync(enrollmentId, user);
                if (result.Success)
                {
                    return this.OkResponse(result);
                }
                else
                {
                    return this.BadRequestResponse(result.Message ?? "Failed to download certificate.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading certificate {EnrollmentId}", enrollmentId);
                return this.BadRequestResponse(ex.Message);
            }
        }
    }
}