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
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(IEnrollmentService enrollmentService, ILogger<EnrollmentController> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        [HttpPost("getCourse/{courseId}")]
        [EndpointDescription("Check course enrollment status")]
        [RequireAction("ENROLLMENT_READ")]
        public async Task<IActionResult> GetCourseByCourseId(Guid courseId)
        {
            try
            {
                var result = await _enrollmentService.GetCourseAsync(courseId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(result.Message ?? "Failed to get enrollment.");
                }
                return this.OkResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollment by course ID.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpGet("get-last-date-Course/{courseId}")]
        [EndpointDescription("Check last date of course")]
        [RequireAction("ENROLLMENT_READ")]
        public async Task<IActionResult> CheckLastDateCourse(Guid courseId)
        {
            try
            {
                var result = await _enrollmentService.CheckLastDateCourseAsync(courseId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(result.Message ?? "Failed to get last date of course.");
                }
                return this.OkResponse(result.LastDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last date of course by course ID.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("updateProgress")]
        [RequireAction("ENROLLMENT_UPDATE")]
        public async Task<IActionResult> UpdateProgress(UpdateProgressRequest request)
        {
            try
            {
                var result = await _enrollmentService.UpdateProgressAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(result.Message ?? "Failed to update progress.");
                }
                return this.OkResponse(result.Enrollment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating progress.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("update-current-view")]
        [RequireAction("ENROLLMENT_UPDATE")]
        public async Task<IActionResult> UpdateCurrentView(UpdateCurrentViewRequest request)
        {
            try
            {
                var result = await _enrollmentService.UpdateCurrentView(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(result.Message ?? "Failed to update current view.");
                }
                return this.OkResponse(result.Enrollment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating current view.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpGet("lessons-completed/{enrollmentId}")]
        [RequireAction("ENROLLMENT_READ")]
        public async Task<IActionResult> GetLessonsCompletedByEnrollmentId(Guid enrollmentId)
        {
            try
            {
                var result = await _enrollmentService.GetLessonsCompletedByEnrollmentIdAsync(enrollmentId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(result.Message ?? "Failed to get lessons completed.");
                }
                return this.OkResponse(result.CompletedLessonIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lessons completed by enrollment ID.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("enroll/{courseId}")]
        [RequireAction("ENROLLMENT_CREATE")]
        public async Task<IActionResult> EnrollInCourse(Guid courseId)
        {
            try
            {
                var result = await _enrollmentService.EnrollInCourseAsyncWithoutGrpc(courseId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(result.Message ?? "Failed to enroll in course.");
                }
                return
                    this.CreatedResponse(
                        result.Enrollment,
                        $"/enrollment/get/{result.Enrollment.Id}"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling in course.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("update")]
        [RequireAction("ENROLLMENT_UPDATE")]
        public async Task<IActionResult> UpdateEnrollmentStatus(UpdateEnrollmentRequest request)
        {
            try
            {
                var result = await _enrollmentService.UpdateEnrollmentStatusAsync(request);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to update enrollment status.");
                }
                return this.OkResponse(result.Enrollment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating enrollment status.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpGet("my-courses")]
        [RequireAction("ENROLLMENT_READ")]
        [Authorize]
        public async Task<IActionResult> GetMyCourses([FromQuery] GetMyCourseRequest request)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                {
                    return this.Unauthorized("User identifier claim is missing or invalid.");
                }

                // Call service to get courses
                var result = await _enrollmentService.GetMyCoursesAsync(userId, request);

                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to get my courses.");
                }

                return this.OkResponse(result.Courses);
            }
            catch (Exception)
            {
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

    }
}
