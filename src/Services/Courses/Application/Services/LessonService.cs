using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Courses.Domain.Enums;
using Codemy.EnrollmentsProto;
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
        private readonly EnrollmentService.EnrollmentServiceClient _enrollmentClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LessonService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<LessonService> logger,
            IRepository<Lesson> lessonRepository,
            IRepository<Module> moduleRepository,
            IRepository<Course> courseRepository,
            IdentityService.IdentityServiceClient client,
            EnrollmentService.EnrollmentServiceClient enrollmentServiceClient,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _lessonRepository = lessonRepository;
            _moduleRepository = moduleRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _client = client;
            _enrollmentClient = enrollmentServiceClient;
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
                .FindAsync(l => l.moduleId == request.moduleId && l.orderIndex == request.orderIndex && !l.IsDeleted);
            if (existingLesson.Any())
                {
                _logger.LogError("Lesson with order index {OrderIndex} already exists for module ID {ModuleId}.", request.orderIndex, request.moduleId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Lesson with the same order index already exists in this module."
                };
            } 
            double durationInSeconds;

            switch (request.lessonType)
            {
                case (int)LessonType.Quiz:  
                case (int)LessonType.Article: 
                    durationInSeconds = (double)(5 * 60); 
                    break;
                case (int)LessonType.Video:
                    durationInSeconds = request.duration;
                    break;
                default:
                    durationInSeconds = (double)(5 * 60);
                    break;
            }  
            var lesson = new Lesson
                {
                    Id = Guid.NewGuid(),
                    title = request.title,
                    contentUrl = request.contentUrl,
                    duration = TimeSpan.FromSeconds(durationInSeconds),
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

        public async Task<LessonListResponse> GetLessons()
        {
            try
            {
                var lessons = await _lessonRepository.GetAllAsync(); 
                var filteredLessons = lessons.Where(l => !l.IsDeleted);
                return new LessonListResponse
                {
                    Success = true,
                    Message = "Get list lesson successfully.",
                    Lessons = filteredLessons.ToList()
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
            if(lesson.IsDeleted)
                {
                _logger.LogError("Lesson with ID {LessonId} is deleted.", lessonId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Lesson is deleted."
                };
            }
            return new LessonResponse
            {
                Success = true,
                Message = "Lesson retrieved successfully.",
                Lesson = lesson
            };
        }

        public async Task<LessonResponse> DeleteLessonAsync(Guid lessonId)
        {
            var result = await _lessonRepository.GetByIdAsync(lessonId);
            if (result == null)
            {
                return new LessonResponse
                {
                    Success = false,
                    Message = "Lesson not found."
                };
            }
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
            if (userId != result.CreatedBy)
            {
                _logger.LogError("User with ID {UserId} is not authorized to delete lesson ID {LessonId}.", userId, lessonId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "User is not authorized to delete this lesson."
                };
            }
            _lessonRepository.Delete(result);
            var listLessonAfterDelete = await _lessonRepository
                .FindAsync(l => l.moduleId == result.moduleId && l.orderIndex > result.orderIndex);
            foreach (var lesson in listLessonAfterDelete)
            {
                lesson.orderIndex -= 1;
                _lessonRepository.Update(lesson);
            }
            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult <= 0)
            {
                return new LessonResponse
                {
                    Success = false,
                    Message = "Failed to delete lesson due to a database error."
                };
            }
            return new LessonResponse
            {
                Success = true,
                Message = "Lesson deleted successfully.",
                Lesson = result
            };
        }

        public async Task<LessonResponse> UpdateLessonAsync(Guid lessonId, CreateLessonRequest request)
        { 
            var result = await _lessonRepository.GetByIdAsync(lessonId);
            if (result == null)
            {
                return new LessonResponse
                {
                    Success = false,
                    Message = "Lesson not found."
                };
            }

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
            if(userId != result.CreatedBy)
            {
                _logger.LogError("User with ID {UserId} is not authorized to update lesson ID {LessonId}.", userId, lessonId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "User is not authorized to update this lesson."
                };
            }
            var existingLesson = await _lessonRepository
                .FindAsync(l => l.moduleId == request.moduleId && l.orderIndex == request.orderIndex && l.Id != lessonId);
            if (existingLesson.Any())
            {
                Console.WriteLine("Duplicate lesson IDs:");
                foreach (var l in existingLesson)
                {
                    Console.WriteLine(l.Id);
                }

                _logger.LogError(
                    "Lesson with order index {OrderIndex} already exists for module ID {ModuleId}. Duplicate IDs: {Ids}",
                    request.orderIndex,
                    request.moduleId,
                    string.Join(", ", existingLesson.Select(l => l.Id))
                );

                return new LessonResponse
                {
                    Success = false,
                    Message = "Lesson with the same order index already exists in this module."
                };
            }

            double durationInSeconds;

            switch (request.lessonType)
            {
                case (int)LessonType.Quiz:
                case (int)LessonType.Article:
                    durationInSeconds = (double)(5 * 60);
                    break;
                case (int)LessonType.Video:
                    durationInSeconds = request.duration;
                    break;
                default:
                    durationInSeconds = (double)(5 * 60);
                    break;
            }
            result.title = request.title;
            result.contentUrl = request.contentUrl;
            result.isPreview = request.isPreview;
            result.duration = TimeSpan.FromSeconds(durationInSeconds);
            result.orderIndex = request.orderIndex;
            result.lessonType = (LessonType)request.lessonType;
            result.UpdatedAt = DateTime.UtcNow;
            _lessonRepository.Update(result);
            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult <= 0)
            {
                return new LessonResponse
                {
                    Success = false,
                    Message = "Failed to update lesson due to a database error."
                };
            }
            return new LessonResponse
            {
                Success = true,
                Message = "Lesson updated successfully.",
                Lesson = result
            };
        }

        public async Task<LessonResponse> CheckLessonLocked(Guid lessonId)
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
                _logger.LogError("User with ID {UserId} does not exist.", userId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            //check lessonId
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
            var module = await _moduleRepository.GetByIdAsync(lesson.moduleId);
            if (module == null)
            {
                _logger.LogError("Module with ID {ModuleId} does not exist.", lesson.moduleId);
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
            var enrollment = _enrollmentClient.GetCourseWithGrpc(
                    new GetCourseWithGrpcRequest
                    {
                        CourseId = course.Id.ToString(),
                        UserId = userId.ToString()
                    });
            if (!lesson.isPreview && !enrollment.Success)
            {
                _logger.LogError("User with ID {UserId} is not enrolled in course ID {CourseId}.", userId, course.Id);
                return new LessonResponse
                {
                    Success = false,
                    Message = "User is not enrolled in this course."
                };
            }
            if (lesson.orderIndex == 1 && module.order == 1)
            {
                return new LessonResponse
                {
                    Success = true,
                    Message = "Lesson is not locked.",
                    Lesson = lesson // Giả sử lesson không bị khóa
                };
            }
            //check progress
            var currentLesson = await _lessonRepository.GetByIdAsync(Guid.Parse(enrollment.LessonId));
            if (currentLesson == null)
            {
                _logger.LogError("Current lesson with ID {CurrentLessonId} does not exist.", enrollment.LessonId);
                return new LessonResponse
                {
                    Success = false,
                    Message = "Current lesson does not exist."
                };
            }
            var currentModule = await _moduleRepository.GetByIdAsync(currentLesson.moduleId);
            if (currentLesson.moduleId == lesson.moduleId)
            {
                if (currentLesson.orderIndex + 1 < lesson.orderIndex)
                {
                    return new LessonResponse
                    {
                        Success = false,
                        Message = "Lesson is Locked"
                    };
                }
            }
            else
            {
                if (module.order > currentModule.order + 1)
                {
                    return new LessonResponse
                    {
                        Success = false,
                        Message = "Lesson is Locked"
                    };
                }
                else if (module.order == currentModule.order + 1 &&
                         (currentLesson.orderIndex != currentModule.numberOfLessons || lesson.orderIndex != 1))
                {
                    return new LessonResponse
                    {
                        Success = false,
                        Message = "Lesson is Locked"
                    };
                }
            }
            return new LessonResponse
            {
                Success = true,
                Message = "Lesson is not Locked",
                Lesson = lesson
            };
        }
    }
}
