using Codemy.BuildingBlocks.Core;
using Codemy.CoursesProto;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Domain.Entities;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Codemy.Enrollment.Application.Services
{
    internal class WishlistService : IWishlistService
    {
        private readonly ILogger<WishlistService> _logger;
        private readonly IRepository<WishlistItem> _wishlistItemRepository;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CoursesService.CoursesServiceClient _courseClient;

        public WishlistService(
            ILogger<WishlistService> logger,
            IRepository<WishlistItem> wishlistItemRepository,
            IUnitOfWork unitOfWork,
            IdentityService.IdentityServiceClient client,
            CoursesService.CoursesServiceClient courseClient,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _logger = logger;
            _wishlistItemRepository = wishlistItemRepository;
            _client = client;
            _courseClient = courseClient;
            _unitOfWork = unitOfWork; 
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> CheckCourseInWishlist(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new Response
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);

            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new Response
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var existingWishlistItem = await _wishlistItemRepository.FindAsync(w => w.userId == UserId && w.courseId == courseId && !w.IsDeleted);
            if (existingWishlistItem.Any())
            {
                return new Response
                {
                    Success = true,
                    Message = "Course is in the wishlist.",
                    WishlistItem = existingWishlistItem.First()
                };
            }
            else
            {
                return new Response
                {
                    Success = false,
                    Message = "Course is not in the wishlist."
                };
            }
        }

        async Task<Response> IWishlistService.AddToWishlistAsync(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new Response
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new Response
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }
            var courseExists = await _courseClient.GetCourseByIdAsync(
                new GetCourseByIdRequest { CourseId = courseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new Response
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            } 

            var existingWishlistItem = await _wishlistItemRepository.FindAsync(w => w.userId == UserId && w.courseId == courseId && ! w.IsDeleted);
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
                userId = UserId,
                courseId = courseId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = UserId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = UserId
            };
            await _wishlistItemRepository.AddAsync(wishlistItem);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return new Response
                {
                    Success = true,
                    Message = "Course added to wishlist successfully.",
                    WishlistItem = wishlistItem
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

        async Task<WishListResponse> IWishlistService.GetWishlistAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            { 
                return new WishListResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);

            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new WishListResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var wishlistItems = await _wishlistItemRepository.FindAsync(w => w.userId == UserId);
            var wishlistItemsFiltered = wishlistItems.Where(wishlistItems => !wishlistItems.IsDeleted);
            if (wishlistItemsFiltered == null || !wishlistItemsFiltered.Any())
            {
                return new WishListResponse
                {
                    Success = false,
                    Message = "Wishlist is empty or does not exist."
                };
            }
            var wishlistItemDTOs = new List<WishlistItemDTO>();
            foreach (var item in wishlistItemsFiltered)
            {
                var courseResponse = await _courseClient.GetCourseByIdAsync(
                    new GetCourseByIdRequest { CourseId = item.courseId.ToString() }
                );
                if (courseResponse.Exists)
                { 
                    var wishlistItemDTO = new WishlistItemDTO
                    {
                        UserId = item.userId,
                        CourseId = item.courseId,
                        Title = courseResponse.Title, 
                        Description = courseResponse.Description,
                        Thumbnail = courseResponse.Thumbnail
                    };
                    wishlistItemDTOs.Add(wishlistItemDTO);
                }
            }
            if(wishlistItemDTOs.Count == 0)
            {
                return new WishListResponse
                {
                    Success = false,
                    Message = "Wishlist is empty or does not exist."
                };
            }
            return new WishListResponse
            {
                Success = true,
                Message = "Wishlist retrieved successfully.",
                WishlistItems = wishlistItemDTOs.ToList()
            };
        }

        async Task<Response> IWishlistService.RemoveFromWishlistAsync(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new Response
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new Response
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }
            var courseExists = await _courseClient.GetCourseByIdAsync(
                new GetCourseByIdRequest { CourseId = courseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new Response
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            } 
            var existingWishlistItem = await _wishlistItemRepository.FindAsync(w => w.userId == UserId && w.courseId == courseId);
            if (!existingWishlistItem.Any())
            {
                return new Response
                {
                    Success = false,
                    Message = "Course does not exist in the wishlist."
                };
            } 
            var wishlistItem = existingWishlistItem.First();
            _wishlistItemRepository.Delete(wishlistItem);
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
