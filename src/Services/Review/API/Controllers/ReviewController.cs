using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Review.Application.DTOs;
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

        [HttpPost("check-user-review")]
        [RequireAction("REVIEW_READ")]
        [Authorize]
        public async Task<IActionResult> CheckUserReview([FromBody] CheckReviewInCourse request)
        {
            try
            { 
                var result = await _service.CheckUserReviewInCourseAsync(request.CourseId, request.ReviewId);
                if (!result.success)
                {
                    return this.NotFoundResponse(result.message ?? "Review not found.");
                }
                return this.OkResponse(result.review);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
        }

        [HttpDelete("user-review")]
        [RequireAction("REVIEW_DELETE")]
        [Authorize]
        public async Task<IActionResult> DeleteUserReview([FromBody] CheckReviewInCourse request)
        {
            try
            {
                var result = await _service.DeleteUserReviewAsync(request.CourseId, request.ReviewId);
                if (!result.success)
                {
                    return this.NotFoundResponse(result.message ?? "Review not found.");
                }
                return this.OkResponse(result.review);
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

        [HttpPost("course/{courseId}/reviews/{reviewId}/reply")]
        [Authorize]
        [SwaggerOperation(Summary = "Reply to a review", Description = "Allows an instructor to reply to a specific review")]
        [RequireAction("REVIEW_REPLY")]
        public async Task<IActionResult> ReplyReview(Guid courseId, Guid reviewId, [FromBody] ReplyReviewRequest request)
        {
            try
            {
                var instructorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _service.ReplyToReviewAsync(courseId, reviewId, instructorId, request.Reply);
                if (!result.success)
                    return this.BadRequest(result);
                return this.OkResponse(result);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
        }

    }
}
