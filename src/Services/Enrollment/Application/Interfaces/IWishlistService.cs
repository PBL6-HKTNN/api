using Codemy.Enrollment.Domain.Entities;

namespace Codemy.Enrollment.Application.Interfaces
{
    public interface IWishlistService
    {
        Task<Response> AddToWishlistAsync(Guid courseId);
        Task<WishListResponse> GetWishlistAsync();
        Task<Response> RemoveFromWishlistAsync(Guid courseId);
    }

    public class WishListResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<WishlistItemDTO>? WishlistItems { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public WishlistItem? WishlistItem { get; set; }
    }

    public class WishlistItemDTO
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public string Thumbnail { get; set; }

    }
}
