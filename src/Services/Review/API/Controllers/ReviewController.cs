using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Review.Application.Interfaces;
using Codemy.Review.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Codemy.Review.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        [RequireAction("REVIEW_CREATE")]
        [SwaggerOperation(Summary = "Create a new review", Description = "Submit a new review for a course")]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _service.AddReviewAsync(request, userId);
                return this.OkResponse(new { message = "Review created successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
        }

        [HttpGet("course/{courseId}")]
        [RequireAction("REVIEW_READ")]
        [Authorize]
        [SwaggerOperation(Summary = "Get reviews by course ID", Description = "Retrieve all reviews for a specific course")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            try
            {
                var reviews = await _service.GetReviewsByCourseIdAsync(courseId);
                return this.OkResponse(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
        }

        [HttpGet("course/{courseId}/average")]
        [RequireAction("REVIEW_READ")]
        [Authorize]
        [SwaggerOperation(Summary = "Get average rating by course ID", Description = "Retrieve the average rating for a specific course")]
        public async Task<IActionResult> GetAverage(Guid courseId)
        {
            try
            {
                var avg = await _service.GetAverageRatingAsync(courseId);
                return this.OkResponse(new { courseId, averageRating = avg });
            }
            catch(KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
        }
    }
}
