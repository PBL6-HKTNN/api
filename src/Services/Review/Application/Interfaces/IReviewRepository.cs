using Codemy.BuildingBlocks.Core;
using ReviewEntity = Codemy.Review.Domain.Entities.Review;

namespace Codemy.Review.Application.Interfaces
{
    public interface IReviewRepository : IRepository<ReviewEntity>
    {
        Task<IEnumerable<ReviewEntity>> GetByCourseIdAsync(Guid courseId);
        Task<double> GetAverageRatingAsync(Guid courseId);
    }
}
