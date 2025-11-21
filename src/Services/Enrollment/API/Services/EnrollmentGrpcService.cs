using Codemy.Enrollment.Application.Interfaces;
using Codemy.EnrollmentsProto;
namespace Codemy.Enrollment.API.Services
{
    public class EnrollmentGrpcService : EnrollmentService.EnrollmentServiceBase
    {
        private readonly IEnrollmentService _enrollmentService;
        public EnrollmentGrpcService(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public override async Task<CreateEnrollmentResponse> CreateEnrollment(CreateEnrollmentRequest request, Grpc.Core.ServerCallContext context)
        {
            var result = await _enrollmentService.EnrollInCourseAsync(Guid.Parse(request.CourseId), Guid.Parse(request.UserId));
            return new CreateEnrollmentResponse
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty,
            };
        }

        public override async Task<GetCourseWithGrpcResponse> GetCourseWithGrpc(GetCourseWithGrpcRequest request, Grpc.Core.ServerCallContext context)
        {
            var result = await _enrollmentService.GetCourseWithGrpc(Guid.Parse(request.CourseId), Guid.Parse(request.UserId));
            return new GetCourseWithGrpcResponse
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty,
                LessonId = result.Enrollment?.lessonId.ToString() ?? string.Empty,
            };
        }

        public override async Task<CheckResponse> CheckEnrollments(CheckRequest request, Grpc.Core.ServerCallContext context)
        {
            var checkRequest = new CheckEnrollmentsRequest
            {
                UserId = Guid.Parse(request.UserId),
                CourseIds = request.CourseIds.Select(id => Guid.Parse(id)).ToList()
            };
            var result = await _enrollmentService.CheckEnrollmentsAsync(checkRequest);
            return new CheckResponse
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty,
                EnrolledCourseIds = { result.EnrolledCourseIds }
            };
        }
    }
}
