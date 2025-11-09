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
            var result = await _enrollmentService.EnrollInCourseAsync(Guid.Parse(request.CourseId));
            return new CreateEnrollmentResponse
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty, 
            };
        }
    }
}
