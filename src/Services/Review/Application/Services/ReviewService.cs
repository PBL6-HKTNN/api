using Codemy.Review.Application.DTOs;
using Codemy.Review.Application.Interfaces;
using Codemy.Review.Application.Requests;
using Codemy.Review.Domain.Entities;
using ReviewEntity = Codemy.Review.Domain.Entities.Review;

namespace Codemy.Review.Application.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _repo;

        public ReviewService(IReviewRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(CreateReviewRequest request)
        {
            var review = new ReviewEntity
            {
                courseId = request.CourseId,
                userId = request.UserId,
                rating = request.Rating,
                comment = request.Comment
            };

            await _repo.AddAsync(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetByCourseAsync(Guid courseId)
        {
            var reviews = await _repo.GetByCourseIdAsync(courseId);
            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                CourseId = r.courseId,
                UserId = r.userId,
                Rating = r.rating,
                Comment = r.comment,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<double> GetAverageAsync(Guid courseId)
        {
            return await _repo.GetAverageRatingAsync(courseId);
        }
    }
}
