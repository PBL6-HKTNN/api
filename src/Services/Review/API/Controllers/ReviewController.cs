using Codemy.Review.Application.Requests;
using Codemy.Review.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.Review.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _service;

        public ReviewController(ReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            await _service.CreateAsync(request);
            return Ok(new { message = "Review created successfully" });
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            var reviews = await _service.GetByCourseAsync(courseId);
            return Ok(reviews);
        }

        [HttpGet("course/{courseId}/average")]
        public async Task<IActionResult> GetAverage(Guid courseId)
        {
            var avg = await _service.GetAverageAsync(courseId);
            return Ok(new { courseId, averageRating = avg });
        }
    }
}
