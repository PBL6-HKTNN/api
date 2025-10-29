using Codemy.Review.Domain.Entities;

namespace Codemy.Review.Application.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync(Domain.Entities.Review review);
        Task<IEnumerable<Domain.Entities.Review>> GetByCourseIdAsync(Guid courseId);
        Task<double> GetAverageRatingAsync(Guid courseId);
    }
}
