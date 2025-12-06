using Codemy.Review.Application.DTOs;
using Codemy.Review.Application.Requests;

namespace Codemy.Review.Application.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(CreateReviewRequest request, Guid userId);
        Task<IEnumerable<ReviewDto>> GetReviewsByCourseIdAsync(Guid courseId);
        Task<double> GetAverageRatingAsync(Guid courseId);
        Task<ReviewResponse> CheckUserReviewInCourseAsync(Guid courseId, Guid reviewId);
        Task<ReviewResponse> DeleteUserReviewAsync(Guid courseId, Guid reviewId);
        Task<ReviewResponse> ReplyToReviewAsync(Guid courseId, Guid reviewId, Guid instructorId, string reply);

    }

    public class ReviewResponse
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public ReviewDto? review { get; set; }
    }
}