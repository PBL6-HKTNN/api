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
                return new GetCourseByIdResponse
                {
                    Exists = false
                };
            }
            return new GetCourseByIdResponse
            {
                Exists = true,
                CourseId = course.Course.Id.ToString(),
                InstructorId = course.Course.instructorId.ToString(),
                Title = course.Course.title,
                Description = course.Course.description,
                Thumbnail = course.Course.thumbnail
            };
        }
    }
}
