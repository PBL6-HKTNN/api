﻿using Microsoft.AspNetCore.Mvc; 
using Codemy.Enrollment.Application.Interfaces;
using Codemy.BuildingBlocks.Core;  
namespace Codemy.Enrollment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        [HttpPost("add/{courseId}")]
        public async Task<IActionResult> AddToWishlist(Guid courseId)
        { 
            try
            {
                var result = await _wishlistService.AddToWishlistAsync(courseId);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to add course to wishlist.");
                }
                return 
                    this.CreatedResponse(
                        result.WishlistItem, 
                        $"/wishlist/get"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course to wishlist.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetWishlist()
        {
            try
            {
                var result = await _wishlistService.GetWishlistAsync();
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to retrieve wishlist.");
                }
                return this.OkResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wishlist.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("remove/{courseId}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid courseId)
        { 
            try
            {
                var result = await _wishlistService.RemoveFromWishlistAsync(courseId);
                if(!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to remove course from wishlist.");
                }
                return this.OkResponse("Course removed from wishlist successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing course from wishlist.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
