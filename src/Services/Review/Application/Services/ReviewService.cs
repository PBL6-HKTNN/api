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
                comment = request.Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedBy = userId
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
                if (r.IsDeleted) continue;
                var userResponse = await _client.GetUserByIdAsync(new GetUserByIdRequest { UserId = r.userId.ToString() });
                reviewDtos.Add(new ReviewDto
                {
                    Id = r.Id,
                    Name = userResponse.Name,
                    CourseId = r.courseId,
                    UserId = r.userId,
                    Rating = r.rating,
                    Comment = r.comment,
                    CreatedAt = r.CreatedAt,
                    Reply = r.reply,
                    RepliedBy = r.repliedBy,
                    RepliedAt = r.repliedAt
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

        public async Task<ReviewResponse> CheckUserReviewInCourseAsync(Guid courseId, Guid reviewId)
        {
            var reviews = await _repo.FindAsync(r => r.courseId == courseId && r.Id == reviewId && !r.IsDeleted);
            if (reviews.Count == 0)
            {
                return new ReviewResponse
                {
                    success = false,
                    message = "Review not found"
                };
            }
            var review = reviews.First();
            return new ReviewResponse
            {
                success = true,
                review = new ReviewDto
                {
                    Id = review.Id,
                    CourseId = review.courseId,
                    UserId = review.userId,
                    Rating = review.rating,
                    Comment = review.comment,
                    CreatedAt = review.CreatedAt
                }
            };
        }

        public async Task<ReviewResponse> DeleteUserReviewAsync(Guid courseId, Guid reviewId)
        {
             var review = await CheckUserReviewInCourseAsync(courseId, reviewId);
             if (!review.success)
             {
                 return new ReviewResponse
                 {
                     success = false,
                     message = "Review not found"
                 };
             }
             var reviewEntity = await _repo.GetByIdAsync(review.review!.Id);
             _repo.Delete(reviewEntity);
             var result = await _unitOfWork.SaveChangesAsync();
             if (result > 0)
             {
                return new ReviewResponse
                {
                    success = true,
                    message = "Review deleted successfully"
                };
             }
             return new ReviewResponse
             {
                 success = false,
                 message = "Failed to delete review"
             };
        }

        public async Task<ReviewResponse> ReplyToReviewAsync(Guid courseId, Guid reviewId, Guid instructorId, string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
                return new ReviewResponse { success = false, message = "Reply cannot be empty or whitespace." };
            // 1. Validate course
            var course = await _courseClient.GetCourseByIdAsync(new GetCourseByIdRequest
            {
                CourseId = courseId.ToString()
            });

            if (!course.Exists)
                return new ReviewResponse { success = false, message = "Course not found" };

            // 2. Check instructor is owner
            if (course.InstructorId != instructorId.ToString())
                return new ReviewResponse { success = false, message = "User is not course instructor" };

            // 3. Check review
            var review = await _repo.GetByIdAsync(reviewId);
            if (review == null || review.IsDeleted)
                return new ReviewResponse { success = false, message = "Review not found" };

            if(review.courseId != courseId)
                return new ReviewResponse { success = false, message = "Review does not belong to the specified course" };

            // 4. Add reply
            review.reply = reply;
            review.repliedBy = instructorId;
            review.repliedAt = DateTime.UtcNow;
            review.UpdatedBy = instructorId;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new ReviewResponse { success = true, message = "Reply added successfully" };
        }
    }
}
