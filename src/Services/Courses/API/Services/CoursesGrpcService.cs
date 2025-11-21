using Codemy.Courses.Application.DTOs;
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
                Thumbnail = course.Course.thumbnail,
                Price = course.Course.price.ToString(),
            };
        }

        public override async Task<GetValidateResponse> ValidateCourseAsync(GetValidateRequest request, Grpc.Core.ServerCallContext context)
        {
            ValidateCourseRequest validateCourseRequest = new ValidateCourseRequest
            {
                CourseId = Guid.Parse(request.CourseId),
                LessonId = Guid.Parse(request.LessonId)
            };
            var validate = await _courseService.ValidateCourseAsync(validateCourseRequest);
            if (validate.Success)
            {
                return new GetValidateResponse { Validate = true , IsLastLesson = validate.isLastLesson ?? false};
            }
            return new GetValidateResponse { Validate = false };
        }

        public override async Task<GetAllCoursesForIndexingResponse> GetAllCoursesForIndexing(Google.Protobuf.WellKnownTypes.Empty request, Grpc.Core.ServerCallContext context)
        {
            var courses = await _courseService.GetCoursesAsync();
            var response = new GetAllCoursesForIndexingResponse();
            response.Courses.AddRange(courses.Select(course => new CourseIndexDto
            {
                Id = course.Id.ToString(),
                InstructorId = course.instructorId.ToString(),
                Title = course.title,
                Description = course.description,
                Thumbnail = course.thumbnail,
                Status = (int)course.status,
                DurationTicks = course.duration.Ticks,
                Price = course.price.ToString(),
                Level = (int)course.level,
                NumberOfModules = course.numberOfModules,
                CategoryId = course.categoryId.ToString(),
                Language = course.language,
                NumberOfReviews = course.numberOfReviews,
                AverageRating = course.averageRating.ToString()
            }));

            return response;
        }
    }
}
