using Codemy.ReviewProto;
using Codemy.Review.Application.Interfaces;
using Grpc.Core;
using Codemy.Review.Application.DTOs;

namespace Codemy.Review.API.Services
{
    public class ReviewServiceGrpc : ReviewService.ReviewServiceBase
    {
        private readonly IReviewService _reviewService;

        public ReviewServiceGrpc(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public override async Task<CheckReviewInCourseResponse> CheckReviewInCourse(CheckReviewInCourseRequest request, ServerCallContext context)
        {
            var hasReviewed = await _reviewService.CheckUserReviewInCourseAsync(Guid.Parse(request.CourseId), Guid.Parse(request.ReviewId));
            return new CheckReviewInCourseResponse {
                Success = hasReviewed.success,
                Message = hasReviewed.message ?? string.Empty
            };
        }

        public override async Task<CheckReviewInCourseResponse> DeleteUserReview(CheckReviewInCourseRequest request, ServerCallContext context)
        {
            var result = await _reviewService.DeleteUserReviewAsync(Guid.Parse(request.CourseId), Guid.Parse(request.ReviewId));
            return new CheckReviewInCourseResponse {
                Success = result.success,
                Message = result.message ?? string.Empty
            };
        }
    }
}
