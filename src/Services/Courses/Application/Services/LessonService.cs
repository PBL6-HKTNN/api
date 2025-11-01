using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Courses.Domain.Enums;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Codemy.Courses.Application.Services
{
    internal class LessonService : ILessonService
    {
        private readonly ILogger<LessonService> _logger;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Module> _moduleRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LessonService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<LessonService> logger,
            IRepository<Lesson> lessonRepository,
            IRepository<Module> moduleRepository,
            IRepository<Course> courseRepository,
            IdentityService.IdentityServiceClient client,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _lessonRepository = lessonRepository;
            _moduleRepository = moduleRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<LessonResponse> CreateLessonAsync(CreateLessonRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new LessonResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = userId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("Instructor with ID {InstructorId} does not exist.", userId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var module = await _moduleRepository.GetByIdAsync(request.moduleId);
            if (module == null)
            {
                _logger.LogError("Module with ID {ModuleId} does not exist.", request.moduleId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Module does not exist."
                };
            }
            var course = await _courseRepository.GetByIdAsync(module.courseId);
            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", module.courseId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            if (userId != course.instructorId)
            {
                _logger.LogError("User with ID {UserId} is not authorized to create lesson for module ID {ModuleId}.", userId, request.moduleId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "User is not authorized to create lesson for this module."
                };
            }
            var existingLesson = await _lessonRepository
                .FindAsync(l => l.moduleId == request.moduleId && l.orderIndex == request.orderIndex);
            if (existingLesson.Any())
                {
                _logger.LogError("Lesson with order index {OrderIndex} already exists for module ID {ModuleId}.", request.orderIndex, request.moduleId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Lesson with the same order index already exists in this module."
                };
            } 
            int durationInMinutes;

            switch (request.lessonType)
            {
                case (int)LessonType.Quiz:  
                case (int)LessonType.Article: 
                    durationInMinutes = 5; 
                    break;
                case (int)LessonType.Video:
                    durationInMinutes = await GetVideoDurationInMinutes(request.contentUrl);
                    break;
                default:
                    durationInMinutes = 5;
                    break;
            }  
            var lesson = new Lesson
                {
                    Id = Guid.NewGuid(),
                    title = request.title,
                    contentUrl = request.contentUrl,
                    duration = TimeSpan.FromMinutes(durationInMinutes),
                    moduleId = request.moduleId,
                    orderIndex = request.orderIndex,
                    isPreview = request.isPreview,
                    lessonType = (LessonType)request.lessonType,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = userId
                };
            await _lessonRepository.AddAsync(lesson);
            course.duration += lesson.duration;
            module.duration += lesson.duration;
            module.numberOfLessons += 1;
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to create lesson for Module ID {ModuleId}.", request.moduleId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Failed to create lesson due to a database error."
                };
            }
            return new LessonResponse
            {
                Success = true,
                Message = "Lesson created successfully.",
                Lesson = lesson
            };
        }

        


    private async Task<int> GetVideoDurationInMinutes(string contentUrl)
        {
        //var account = new Account(
        //    "your_cloud_name",
        //    "your_api_key",
        //    "your_api_secret"
        //);

        //var cloudinary = new Cloudinary(account);
        //var result = await cloudinary.GetResourceAsync(new GetResourceParams(publicId)
        //{
        //    ResourceType = ResourceType.Video
        //});

        //return result.Duration; // tính bằng giây
        return 10; // Giả sử video có độ dài 10 phút
        }

        public async Task<LessonListResponse> GetLessons()
        {
            try
            {
                var lessons = await _lessonRepository.GetAllAsync(); 
                return new LessonListResponse
                {
                    Success = true,
                    Message = "Get list lesson successfully.",
                    Lessons = lessons.ToList()
                };
            }
            catch (Exception e)
            {
                return new LessonListResponse
                {
                    Success = false,
                    Message = e.Message,
                    Lessons = new List<Lesson>()
                };
            }
        }

        public async Task<LessonResponse> GetLessonById(Guid lessonId)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                _logger.LogError("Lesson with ID {LessonId} does not exist.", lessonId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Lesson does not exist."
                };
            }
            return new LessonResponse
            {
                Success = true,
                Message = "Lesson retrieved successfully.",
                Lesson = lesson
            };
        }
    }
}
