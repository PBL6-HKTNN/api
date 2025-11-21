using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Courses.Domain.Enums;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Codemy.SearchProto;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Module = Codemy.Courses.Domain.Entities.Module;
using Microsoft.EntityFrameworkCore;

namespace Codemy.Courses.Application.Services
{
    internal class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Module> _moduleRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly CourseIndexService.CourseIndexServiceClient _searchClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CourseService> logger,
            IRepository<Course> courseRepository,
            IRepository<Module> moduleRepository,
            IRepository<Lesson> lessonRepository,
            IRepository<Category> categoryRepository,
            IUnitOfWork unitOfWork,
            IdentityService.IdentityServiceClient client,
            CourseIndexService.CourseIndexServiceClient searchClient
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _lessonRepository = lessonRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _client = client;
            _searchClient = searchClient;
        }
        public async Task<CourseReponse> CreateCourseAsync(CreateCourseRequest request)
        {
            var user =  await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = request.instructorId.ToString() }
            ); 

            if (!user.Exists)
            {
                _logger.LogError("Instructor with ID {InstructorId} does not exist.", request.instructorId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Instructor does not exist."
                };
            }
            var category = await _categoryRepository.GetByIdAsync(request.categoryId);
            if (category == null)
            {
                _logger.LogError("Category with ID {CategoryId} does not exist.", request.categoryId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Category does not exist."
                };
            }
            var course = new Course
            {
                Id = Guid.NewGuid(),
                title = request.title,
                description = request.description,
                thumbnail = request.thumbnail,
                categoryId = request.categoryId,
                price = request.price,
                language = request.language,
                numberOfModules = 0,
                level = request.level,
                instructorId = request.instructorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = request.instructorId,
                UpdatedBy = request.instructorId
            };
            await _courseRepository.AddAsync(course);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to create course for Instructor ID {InstructorId}.", request.instructorId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Failed to create course."
                };
            }
            _logger.LogInformation("Course {CourseTitle} created successfully for Instructor ID {InstructorId}.", course.title, request.instructorId);
            
            try
            {
                await _searchClient.IndexCourseAsync(new CourseIndexRequest
                {
                    Id = course.Id.ToString(),
                    InstructorId = course.instructorId.ToString(),
                    Title = course.title,
                    Description = course.description,
                    Thumbnail = course.thumbnail,
                    Status = (int)course.status,
                    Duration = course.duration.ToString(),
                    Price = (double)course.price,
                    Level = (int)course.level,
                    NumberOfModules = course.numberOfModules,
                    CategoryId = course.categoryId.ToString(),
                    Language = course.language,
                    NumberOfReviews = course.numberOfReviews,
                    AverageRating = (double)course.averageRating
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to index course {CourseId} in search service after creation.", course.Id);
            }

            return new CourseReponse
            {
                Success = true,
                Message = "Course created successfully.",
                Course = course
            };
        }

        public async Task<CourseReponse> DeleteCourseAsync(Guid courseId)
        {
            var result = await _courseRepository.GetByIdAsync(courseId);
            if (result == null)
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course not found."
                };
            }
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new CourseReponse
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
                _logger.LogError("User with ID {UserId} is not authorized to delete course ID {CourseId}.", userId, courseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "User is not authorized to delete this course."
                };
            }
            _courseRepository.Delete(result);
            var moduleList = await _moduleRepository
                .FindAsync(m => m.courseId == result.Id);
            foreach (var module in moduleList)
            {
                _moduleRepository.Delete(module);
                var listLessonAfterDelete = await _lessonRepository
                .FindAsync(l => l.moduleId == module.Id);
                foreach (var lesson in listLessonAfterDelete)
                {
                    _lessonRepository.Delete(lesson);
                }
            }
            
            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult <= 0)
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "Failed to delete course due to a database error."
                };
            }
            return new CourseReponse
            {
                Success = true,
                Message = "Course deleted successfully.",
            };
        }

        public async Task<CourseReponse> GetCourseByIdAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            if(course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} is deleted.", courseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course is deleted."
                };
            }
            return new CourseReponse
            {
                Success = true,
                Message = "Course retrieved successfully.",
                Course = course
            };
        }

        public async Task<ResourceDtoResponse> GetLessonByCourseIdAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new ResourceDtoResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            if (course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} is deleted.", courseId);
                return new ResourceDtoResponse
                {
                    Success = false,
                    Message = "Course is deleted."
                };
            }
            var modules = await _moduleRepository.GetAllAsync(m => m.courseId == courseId);
            var filteredModules = modules.Where(m => !m.IsDeleted);
            var moduleSort = filteredModules.OrderBy(m => m.order);
            CourseDto courseDto = new CourseDto
            {
                course = course,
                module = new List<ModuleDto>()
            };
            foreach (var module in moduleSort)
            {
                var lessons = await _lessonRepository.GetAllAsync(l => l.moduleId == module.Id);
                var filteredLessons = lessons.Where(l => !l.IsDeleted).ToList();
                var lessonSort = filteredLessons.OrderBy(l => l.orderIndex).ToList();
                ModuleDto moduleDto = new ModuleDto
                {
                    Id = module.Id,
                    title = module.title,
                    duration = module.duration,
                    numberOfLessons = module.numberOfLessons,
                    order = module.order,
                    lessons = lessonSort
                };
                courseDto.module.Add(moduleDto);
            }
            return new ResourceDtoResponse
            {
                Success = true,
                Message = "Course and lessons retrieved successfully.",
                Course = courseDto
            };
        }

        public async Task<ModuleListResponse> GetModuleByCourseIdAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new ModuleListResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            if (course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} is deleted.", courseId);
                return new ModuleListResponse
                {
                    Success = false,
                    Message = "Course is deleted."
                };
            }
            var modules = await _moduleRepository.GetAllAsync(m => m.courseId == courseId);
            var filteredModules = modules.Where(m => !m.IsDeleted);
            return new ModuleListResponse
            {
                Success = true,
                Message = "Course retrieved successfully.",
                Modules = filteredModules.ToList()
            };
        }

        public async Task<CourseReponse> UpdateCourseAsync(Guid courseId, CreateCourseRequest request)
        {

            var result = await _courseRepository.GetByIdAsync(courseId);
            if (result == null)
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course not found."
                };
            }

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new CourseReponse
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
                _logger.LogError("User with ID {UserId} is not authorized to update course ID {CourseId}.", userId, courseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "User is not authorized to update this course."
                };
            }
            var category = await _categoryRepository.GetByIdAsync(request.categoryId);
            if (category == null)
            {
                _logger.LogError("Category with ID {CategoryId} does not exist.", request.categoryId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Category does not exist."
                };
            }
            result.title = request.title;
            result.description = request.description;
            result.thumbnail = request.thumbnail;
            result.categoryId = request.categoryId;
            result.price = request.price;
            result.language = request.language;
            result.level = request.level;
            result.UpdatedAt = DateTime.UtcNow;
            _courseRepository.Update(result);
            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult <= 0)
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "Failed to update course due to a database error."
                };
            }
            return new CourseReponse
            {
                Success = true,
                Message = "Course updated successfully.",
                Course = result
            };
        }

        public async Task<ValidateCourseResponse> ValidateCourseAsync(ValidateCourseRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);

            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", request.CourseId);
                return new ValidateCourseResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            if (course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} is deleted.", request.CourseId);
                return new ValidateCourseResponse
                {
                    Success = false,
                    Message = "Course is deleted."
                };
            }
            var modules = await _moduleRepository.GetAllAsync(m => m.courseId == request.CourseId);
            var filteredModules = modules.Where(m => !m.IsDeleted);
            foreach (var module in filteredModules)
            {
                var lessonExists = (await _lessonRepository.GetAllAsync(l => l.moduleId == module.Id && !l.IsDeleted))
                    .Any(l => l.Id == request.LessonId);

                if (lessonExists)
                {
                    var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
                    if (module.order == course.numberOfModules && lesson.orderIndex == module.numberOfLessons)
                    {
                        return new ValidateCourseResponse
                        {
                            Success = true,
                            Message = "Lesson belongs to this course",
                            isLastLesson = true
                        };
                    }

                    return new ValidateCourseResponse
                    {
                        Success = true,
                        Message = "Lesson belongs to this course",
                        isLastLesson = false
                    };
                }
            }
            return new ValidateCourseResponse
            {
                Success = false,
                Message = "Lesson does not belong to this course"
            };
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(
            Guid? categoryId = null,
            string? language = null,
            string? level = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _courseRepository.Query();

            if (categoryId.HasValue)
                query = query.Where(c => c.categoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(language))
                query = query.Where(c => c.language == language);

            if (!string.IsNullOrEmpty(level) && Enum.TryParse(level, out Level parsedLevel))
                query = query.Where(c => c.level == parsedLevel);

            query = sortBy switch
            {
                "price" => query.OrderBy(c => c.price),
                "rating" => query.OrderByDescending(c => c.averageRating),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            var courses = await query
                .ToListAsync();

            return courses;
        }
    }
}
