using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.EnrollmentsProto;
namespace Codemy.Enrollment.API.Services
{
    public class EnrollmentGrpcService : EnrollmentService.EnrollmentServiceBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentGrpcService> _logger;
        public EnrollmentGrpcService(
            IEnrollmentService enrollmentService,
            ILogger<EnrollmentGrpcService> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
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
            _logger.LogInformation("GetCourseWithGrpc called for CourseId: {CourseId}, UserId: {UserId}, Success: {Success}", request.CourseId, request.UserId, result.Success);
            _logger.LogInformation("Enrollment Details: {Enrollment}", result.Enrollment);
            _logger.LogInformation("LessonId: {LessonId}", result.Enrollment?.lessonId);
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

        public override async Task<GetLastDateCourseResponse> GetLastDateCourse(GetLastDateCoureRequest request, Grpc.Core.ServerCallContext context)
        {
            var result = await _enrollmentService.CheckLastDateCourseAsync(Guid.Parse(request.CourseId));
            return new GetLastDateCourseResponse
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty,
                LastDate = result.LastDate?.ToString("o") ?? string.Empty
            };
        }

        public override async Task<GetListStudentResponse> GetListStudent(GetLastDateCoureRequest request, Grpc.Core.ServerCallContext context)
        {
            var result = await _enrollmentService.GetListStudentsByCourseId(Guid.Parse(request.CourseId));
            var response = new GetListStudentResponse
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty,
            };
            if (result.Students != null)
            {
                response.StudentEmails.AddRange(result.Students);
            }
            return response;
        }

        public override async Task<GetTotalEnrollmentsByCourseIdResponse> GetTotalEnrollmentsByCourseId(GetLastDateCoureRequest request, Grpc.Core.ServerCallContext context)
        {
            var result = await _enrollmentService.GetTotalEnrollmentsByCourseId(Guid.Parse(request.CourseId));
            var response = new GetTotalEnrollmentsByCourseIdResponse
            {
                Success = result.Success,
                Message = result.Message ?? string.Empty,
            };
            if (result.TotalEnrollments != null)
            {
                response.TotalEnrollments = result.TotalEnrollments.TotalEnrollments;
                response.EnrollmentDate.AddRange(result.TotalEnrollments.EnrollmentDate.Select(date => date.ToString("o")));
            }
            return response;
        }
    }
}
