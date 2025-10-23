using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Domain.Entities;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Domain.Entities; 
using Codemy.Identity.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Codemy.Enrollment.Application.Services
{
    internal class WishlistService : IWishlistService
    {
        private readonly ILogger<WishlistService> _logger;
        private readonly IRepository<WishlistItem> _wishlistRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Course> _courseRepository; 
        private readonly IUnitOfWork _unitOfWork; 

        public WishlistService(
            ILogger<WishlistService> logger,
            IRepository<WishlistItem> wishlistRepository,
            IUnitOfWork unitOfWork,
            IRepository<User> userRepository
            )
        {
            _logger = logger;
            _wishlistRepository = wishlistRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork; 
        }

        async Task<Response> IWishlistService.AddToWishlistAsync(Guid userId, Guid courseId)
        {
            var usersById = await _userRepository.FindAsync(u => u.Id == userId);
            var user = usersById.FirstOrDefault();

            if (user == null)
            {
                return new Response
                {
                    Success = false,
                    Message = "User not found."
                };
            } 
            var courseById = await _courseRepository.FindAsync(c => c.Id == courseId);
            var course = courseById.FirstOrDefault();
            if (course == null)
            {
                return new Response
                {
                    Success = false,
                    Message = "Course not found."
                };
            }

            var existingWishlistItem = await _wishlistRepository.FindAsync(w => w.userId == userId && w.courseId == courseId);
            if (existingWishlistItem.Any())
            {
                return new Response
                {
                    Success = false,
                    Message = "Course is already in the wishlist."
                };
            }

            // Create a new wishlist item
            var wishlistItem = new WishlistItem
            {
                userId = userId,
                courseId = courseId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId
            };
            await _wishlistRepository.AddAsync(wishlistItem);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return new Response
                {
                    Success = true,
                    Message = "Course added to wishlist successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course to wishlist.");
                return new Response
                {
                    Success = false,
                    Message = "An error occurred while adding the course to the wishlist."
                };
            }
        }

        async Task<List<WishlistItem>> IWishlistService.GetWishlistAsync(Guid userId)
        {

            var usersById = await _userRepository.FindAsync(u => u.Id == userId);
            var user = usersById.FirstOrDefault();

            if (user == null)
            {
                return new List<WishlistItem>();
            }

            var wishlistItems = await _wishlistRepository.FindAsync(w => w.userId == userId);
            if (wishlistItems == null || !wishlistItems.Any())
            {
                return new List<WishlistItem>();
            }
            try
            {
                return wishlistItems.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wishlist for user {UserId}.", userId);
                return new List<WishlistItem>();
            }
        }

        async Task<Response> IWishlistService.RemoveFromWishlistAsync(Guid userId, Guid courseId)
        {
            var usersById = await _userRepository.FindAsync(u => u.Id == userId);
            var user = usersById.FirstOrDefault();

            if (user == null)
            {
                return new Response
                {
                    Success = false,
                    Message = "User not found."
                };
            }
            var courseById = await _courseRepository.FindAsync(c => c.Id == courseId);
            var course = courseById.FirstOrDefault();
            if (course == null)
            {
                return new Response
                {
                    Success = false,
                    Message = "Course not found."
                };
            }

            var existingWishlistItem = await _wishlistRepository.FindAsync(w => w.userId == userId && w.courseId == courseId);
            if (!existingWishlistItem.Any())
            {
                return new Response
                {
                    Success = false,
                    Message = "Course is not already in the wishlist."
                };
            }

            // Remove the wishlist item
            var wishlistItem = existingWishlistItem.First();
            _wishlistRepository.Delete(wishlistItem);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return new Response
                {
                    Success = true,
                    Message = "Deleted course from wishlist successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course from wishlist.");
                return new Response
                {
                    Success = false,
                    Message = "An error occurred while deleting course from wishlist."
                };
            }

        }
    }
}
