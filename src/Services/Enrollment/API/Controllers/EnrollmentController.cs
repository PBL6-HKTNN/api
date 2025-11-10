using Microsoft.AspNetCore.Mvc;
using Codemy.BuildingBlocks.Core;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Application.DTOs;

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
        public async Task<IActionResult> GetCourseByCourseId(Guid courseId)
    {
        try
        {
                var result = await _enrollmentService.GetCourseAsync(courseId);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to get enrollment.");
                }
                return this.OkResponse(result.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollment by course ID.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> EnrollInCourse(Guid courseId)
        {
            try
            {
                var result = await _enrollmentService.EnrollInCourseAsyncWithoutGrpc(courseId);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to enroll in course.");
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
    }
}
