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
        Task<Response> AddToWishlistAsync(Guid userId, Guid courseId);
        Task<List<WishlistItem>> GetWishlistAsync(Guid userId);
        Task<Response> RemoveFromWishlistAsync(Guid userId, Guid courseId);
    }

    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
