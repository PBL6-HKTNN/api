
using System.Security.Claims;
using Codemy.BuildingBlocks.Core;
using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Infrastructure.Clients;
using Codemy.Enrollment.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentDbContext _context;
    private readonly CourseClient _courseClient;

    public EnrollmentController(
        EnrollmentDbContext context,
        CourseClient courseClient 
    )
    {
        _context = context;
        _courseClient = courseClient;
    }

    [HttpGet("my-courses")]
    [Authorize]
    public async Task<IActionResult> GetMyCourses(int page = 1, int pageSize = 10)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var courseIds = await _context.Enrollments
                .Where(e => e.studentId == userId)
                .Select(e => e.courseId)
                .ToListAsync();

            if (!courseIds.Any())
            {
                return this.OkResponse(new List<CourseDto>());
            }

            var courses = await _courseClient.GetCoursesByIdsAsync(courseIds);

            return this.OkResponse(courses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
