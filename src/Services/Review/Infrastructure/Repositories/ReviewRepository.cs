using Codemy.Review.Application.Interfaces;
using Codemy.Review.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ReviewEntity = Codemy.Review.Domain.Entities.Review;

namespace Codemy.Review.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _context;

        public ReviewRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReviewEntity review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewEntity>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Reviews
                .Where(r => r.courseId == courseId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(Guid courseId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.courseId == courseId)
                .Select(r => r.rating)
                .ToListAsync();

            return ratings.Count == 0 ? 0 : ratings.Average();
        }
    }
}
