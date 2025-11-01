using Codemy.Enrollment.Domain.Entities;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<WishlistItem>? WishlistItems { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public WishlistItem? WishlistItem { get; set; }
    }
}
