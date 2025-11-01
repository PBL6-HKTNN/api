using Codemy.Courses.Application.Interfaces;
using Codemy.CoursesProto;

namespace Codemy.Courses.API.Services
{
    public class CoursesGrpcService : CoursesService.CoursesServiceBase
    {
        private readonly ICourseService _courseService;
        public CoursesGrpcService(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task<GetCourseByIdResponse> GetCourseById(GetCourseByIdRequest request, Grpc.Core.ServerCallContext context)
        {
            var course = await _courseService.GetCourseByIdAsync(Guid.Parse(request.CourseId));
            if (!course.Success)
            {
                Console.WriteLine($"Course {request.CourseId} không tồn tại");
                return new GetCourseByIdResponse
                {
                    Exists = false
                };
            }
            Console.WriteLine($"Course found: {course.Course.title} ({course.Course.description})");
            return new GetCourseByIdResponse
            {
                Exists = true,
                CourseId = course.Course.Id.ToString(),
                InstructorId = course.Course.instructorId.ToString(),
                Title = course.Course.title
            };
        }
    }
}
