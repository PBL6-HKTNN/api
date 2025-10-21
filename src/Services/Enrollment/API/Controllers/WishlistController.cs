using Microsoft.AspNetCore.Mvc; 
using Codemy.Enrollment.Application.Interfaces;
using Codemy.BuildingBlocks.Core;
using Codemy.Enrollment.API.DTOs;
namespace Codemy.Enrollment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WishlistController : ControllerBase
    {
        private IWishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
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
                await _wishlistService.AddToWishlistAsync(request.UserId, request.CourseId);
                return Ok("Course added to wishlist successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course to wishlist.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetWishlist(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }
            try
            {
                var wishlist = await _wishlistService.GetWishlistAsync(userId);
                if (wishlist == null || !wishlist.Any())
                {
                    return NotFound("Wishlist is empty or does not exist.");
                }
                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wishlist.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] AddToWishlistRequest request)
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
                await _wishlistService.RemoveFromWishlistAsync(request.UserId, request.CourseId);
                return Ok("Course removed from wishlist successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing course from wishlist.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
