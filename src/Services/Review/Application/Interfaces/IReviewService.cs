using Codemy.Review.Application.DTOs;

namespace Codemy.Review.Application.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(Requests.CreateReviewRequest request, Guid userId);
        Task<IEnumerable<ReviewDto>> GetReviewsByCourseIdAsync(Guid courseId);
        Task<double> GetAverageRatingAsync(Guid courseId);
    }
}