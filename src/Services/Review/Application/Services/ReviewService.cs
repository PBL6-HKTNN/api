using Codemy.BuildingBlocks.Core;
using Codemy.CoursesProto;
using Codemy.IdentityProto;
using Codemy.Review.Application.DTOs;
using Codemy.Review.Application.Interfaces;
using Codemy.Review.Application.Requests;
using Codemy.Review.Domain.Entities;
using ReviewEntity = Codemy.Review.Domain.Entities.Review;

namespace Codemy.Review.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly CoursesService.CoursesServiceClient _courseClient;

        public ReviewService(IReviewRepository repo, IUnitOfWork unitOfWork, IdentityService.IdentityServiceClient client, CoursesService.CoursesServiceClient courseClient)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _client = client;
            _courseClient = courseClient;
        }

        public async Task AddReviewAsync(CreateReviewRequest request, Guid userId)
        {
            var course = await _courseClient.GetCourseByIdAsync(new GetCourseByIdRequest { CourseId = request.CourseId.ToString() });
            if (!course.Exists)
            {
                throw new KeyNotFoundException("Course not found");
            }
            var review = new ReviewEntity
            {
                courseId = request.CourseId,
                userId = userId,
                rating = request.Rating,
                comment = request.Comment
            };

            await _repo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByCourseIdAsync(Guid courseId)
        {
            var course = await _courseClient.GetCourseByIdAsync(new GetCourseByIdRequest { CourseId = courseId.ToString() });
            if (!course.Exists)
            {
                throw new KeyNotFoundException("Course not found");
            }
            var reviews = await _repo.GetByCourseIdAsync(courseId);

            var reviewDtos = new List<ReviewDto>();
            foreach (var r in reviews)
            {
                var userResponse = await _client.GetUserByIdAsync(new GetUserByIdRequest { UserId = r.userId.ToString() });
                reviewDtos.Add(new ReviewDto
                {
                    Id = r.Id,
                    Name = userResponse.Name,
                    CourseId = r.courseId,
                    UserId = r.userId,
                    Rating = r.rating,
                    Comment = r.comment,
                    CreatedAt = r.CreatedAt
                });
            }
            return reviewDtos;
        }

        public async Task<double> GetAverageRatingAsync(Guid courseId)
        {
            var course = await _courseClient.GetCourseByIdAsync(new GetCourseByIdRequest { CourseId = courseId.ToString() });
            if (!course.Exists)
            {
                throw new KeyNotFoundException("Course not found");
            }
            return await _repo.GetAverageRatingAsync(courseId);
        }
    }
}
