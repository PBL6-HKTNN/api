using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Courses.Domain.Enums;
using Codemy.EnrollmentsProto;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Codemy.IdentityProto;
using Codemy.SearchProto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sprache;
using System.Net.WebSockets;
using System.Security.Claims;
using Module = Codemy.Courses.Domain.Entities.Module;

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
        private readonly EnrollmentService.EnrollmentServiceClient _enrollmentService;
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
            CourseIndexService.CourseIndexServiceClient searchClient,
            EnrollmentService.EnrollmentServiceClient enrollmentService
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
            _enrollmentService = enrollmentService;
        }
        public async Task<CourseReponse> CreateCourseAsync(CreateCourseRequest request)
        {
            var user = await _client.GetUserByIdAsync(
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
                status = Status.Draft,
                price = request.price,
                language = request.language,
                numberOfModules = 0,
                level = request.level,
                instructorId = request.instructorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = request.instructorId,
                UpdatedBy = request.instructorId,
                isRequestedBanned = false,
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

            //try
            //{
            //    await _searchClient.IndexCourseAsync(new CourseIndexRequest
            //    {
            //        Id = course.Id.ToString(),
            //        InstructorId = course.instructorId.ToString(),
            //        Title = course.title,
            //        Description = course.description,
            //        Thumbnail = course.thumbnail,
            //        Status = (int)course.status,
            //        Duration = course.duration.ToString(),
            //        Price = (double)course.price,
            //        Level = (int)course.level,
            //        NumberOfModules = course.numberOfModules,
            //        CategoryId = course.categoryId.ToString(),
            //        Language = course.language,
            //        NumberOfReviews = course.numberOfReviews,
            //        AverageRating = (double)course.averageRating
            //    });
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to index course {CourseId} in search service after creation.", course.Id);
            //}

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
            if (course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} is deleted.", courseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course is deleted."
                };
            }

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                if (course.isRequestedBanned)
                {
                    _logger.LogError("Course with ID {CourseId} is banned.", courseId);
                    return new CourseReponse
                    {
                        Success = false,
                        Message = "Course is banned."
                    };
                }
            }
            else
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? user.FindFirst("sub")?.Value
                               ?? user.FindFirst("userId")?.Value;

                var userId = Guid.Parse(userIdClaim);

                var role = user.FindFirst(ClaimTypes.Role)?.Value
                           ?? user.FindFirst("role")?.Value;
                _logger.LogInformation("User with ID {UserId} and role {Role} is accessing course ID {CourseId}.", userId, role, courseId);
                if (role != "Student")
                {
                    _logger.LogError("Instructor with ID {UserId} is not authorized to access course ID {CourseId}, by {Instructor ID}", userId, courseId, course.instructorId);

                    if (role == "Instructor" && userId != course.instructorId && (course.status != Status.Published || course.isRequestedBanned))
                    {
                        return new CourseReponse
                        {
                            Success = false,
                            Message = "Instructor is not authorized to access this course."
                        };
                    }
                    return new CourseReponse
                    {
                        Success = true,
                        Course = course
                    };
                }

                if (course.status != Status.Published)
                {
                    return new CourseReponse
                    {
                        Success = false,
                        Message = "Course is not published."
                    };
                }

                var response = _enrollmentService.GetCourseWithGrpc(new GetCourseWithGrpcRequest
                {
                    UserId = userId.ToString(),
                    CourseId = course.Id.ToString()
                });
                if (!response.Success && course.isRequestedBanned)
                {
                    _logger.LogError("Course with ID {CourseId} is banned.", courseId);
                    return new CourseReponse
                    {
                        Success = false,
                        Message = "Course is banned."
                    };
                }
            }
            if (course.status != Status.Published)
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course is not published."
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

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                if (course.isRequestedBanned)
                {
                    _logger.LogError("Course with ID {CourseId} is banned.", courseId);
                    return new ResourceDtoResponse
                    {
                        Success = false,
                        Message = "Course is banned."
                    };
                }
            }
            else
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? user.FindFirst("sub")?.Value
                               ?? user.FindFirst("userId")?.Value;

                var userId = Guid.Parse(userIdClaim);

                var role = user.FindFirst(ClaimTypes.Role)?.Value
                           ?? user.FindFirst("role")?.Value;
                _logger.LogInformation("User with ID {UserId} and role {Role} is accessing course ID {CourseId}.", userId, role, courseId);
                if (role != "Student")
                {
                    _logger.LogError("Instructor with ID {UserId} is not authorized to access course ID {CourseId}, by {Instructor ID}", userId, courseId, course.instructorId);

                    if (role == "Instructor" && userId != course.instructorId && (course.status != Status.Published || course.isRequestedBanned))
                    {
                        return new ResourceDtoResponse
                        {
                            Success = false,
                            Message = "Instructor is not authorized to access this course."
                        };
                    }
                    return new ResourceDtoResponse
                    {
                        Success = true,
                        Course = courseDto
                    };
                }

                if (course.status != Status.Published)
                {
                    return new ResourceDtoResponse
                    {
                        Success = false,
                        Message = "Course is not published."
                    };
                }

                var response = _enrollmentService.GetCourseWithGrpc(new GetCourseWithGrpcRequest
                {
                    UserId = userId.ToString(),
                    CourseId = course.Id.ToString()
                });
                if (!response.Success && course.isRequestedBanned)
                {
                    _logger.LogError("Course with ID {CourseId} is banned.", courseId);
                    return new ResourceDtoResponse
                    {
                        Success = false,
                        Message = "Course is banned."
                    };
                }
            }
            if (course.status != Status.Published)
            {
                return new ResourceDtoResponse
                {
                    Success = false,
                    Message = "Course is not published."
                };
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

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                if (course.isRequestedBanned)
                {
                    _logger.LogError("Course with ID {CourseId} is banned.", courseId);
                    return new ModuleListResponse
                    {
                        Success = false,
                        Message = "Course is banned."
                    };
                }
            }
            else
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? user.FindFirst("sub")?.Value
                               ?? user.FindFirst("userId")?.Value;

                var userId = Guid.Parse(userIdClaim);

                var role = user.FindFirst(ClaimTypes.Role)?.Value
                           ?? user.FindFirst("role")?.Value;
                _logger.LogInformation("User with ID {UserId} and role {Role} is accessing course ID {CourseId}.", userId, role, courseId);
                if (role != "Student")
                {
                    _logger.LogError("Instructor with ID {UserId} is not authorized to access course ID {CourseId}, by {Instructor ID}", userId, courseId, course.instructorId);

                    if (role == "Instructor" && userId != course.instructorId && (course.status != Status.Published || course.isRequestedBanned))
                    {
                        return new ModuleListResponse
                        {
                            Success = false,
                            Message = "Instructor is not authorized to access this course."
                        };
                    }
                    return new ModuleListResponse
                    {
                        Success = true,
                        Message = "Course retrieved successfully.",
                        Modules = filteredModules.ToList()
                    };
                }

                if (course.status != Status.Published)
                {
                    return new ModuleListResponse
                    {
                        Success = false,
                        Message = "Course is not published."
                    };
                }

                var response = _enrollmentService.GetCourseWithGrpc(new GetCourseWithGrpcRequest
                {
                    UserId = userId.ToString(),
                    CourseId = course.Id.ToString()
                });
                if (!response.Success && course.isRequestedBanned)
                {
                    _logger.LogError("Course with ID {CourseId} is banned.", courseId);
                    return new ModuleListResponse
                    {
                        Success = false,
                        Message = "Course is banned."
                    };
                }
            }
            if (course.status != Status.Published)
            {
                return new ModuleListResponse
                {
                    Success = false,
                    Message = "Course is not published."
                };
            }
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

        public async Task<IEnumerable<GetCoursesResponse>> GetCoursesAsync(
            Guid? categoryId = null,
            Guid? instructorId = null,
            string? language = null,
            string? level = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            Guid? userId = null;

            if (httpContext?.User != null)
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? httpContext.User.FindFirst("sub")?.Value
                            ?? httpContext.User.FindFirst("userId")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedId))
                {
                    userId = parsedId;
                }
            }

            // Query courses
            var query = _courseRepository.Query();

            if (categoryId.HasValue)
                query = query.Where(c => c.categoryId == categoryId.Value);

            if (instructorId.HasValue)
                query = query.Where(c => c.instructorId == instructorId.Value);

            if (!string.IsNullOrEmpty(language))
                query = query.Where(c => c.language == language);

            if (!string.IsNullOrEmpty(level) && Enum.TryParse(level, out Level parsedLevel))
                query = query.Where(c => c.level == parsedLevel);

            // Sorting
            query = sortBy switch
            {
                "price" => query.OrderBy(c => c.price),
                "rating" => query.OrderByDescending(c => c.averageRating),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            // Pagination
            var skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);

            // Get courses list
            var courses = await query.ToListAsync();

            // Batch check enrollments
            List<string> enrolledCourseIds = new List<string>();
            if (userId.HasValue && courses.Any())
            {
                try
                {
                    var response = await _enrollmentService.CheckEnrollmentsAsync(new CheckRequest
                    {
                        UserId = userId.Value.ToString(),
                        CourseIds = { courses.Select(c => c.Id.ToString()).ToList() }
                    });
                    enrolledCourseIds = response?.EnrolledCourseIds?.ToList() ?? new List<string>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to check enrollments for user {UserId}. Defaulting IsEnrolled to false for all courses.", userId.Value);
                    enrolledCourseIds = new List<string>();
                }
            }

            var result = courses.Select(c => new GetCoursesResponse
            {
                Id = c.Id,
                instructorId = c.instructorId,
                title = c.title,
                description = c.description,
                thumbnail = c.thumbnail,
                status = (int)c.status,
                duration = c.duration,
                price = c.price,
                level = (int)c.level,
                numberOfModules = c.numberOfModules,
                categoryId = c.categoryId,
                language = c.language,
                numberOfReviews = c.numberOfReviews,
                averageRating = c.averageRating,
                IsEnrolled = enrolledCourseIds.Contains(c.Id.ToString()),
                isRequestedBanned = c.isRequestedBanned
            }).ToList();

            // Apply ban filtering rules
            if (!userId.HasValue)
            {
                // Guest ⇒ hide all banned courses
                result = result.Where(c => !c.isRequestedBanned).ToList();
            }
            else
            {
                // Logged-in user rules:
                // Hide only when (not enrolled) AND (isRequestedBanned = true)
                result = result
                    .Where(c => !(c.isRequestedBanned && c.IsEnrolled == false))
                    .ToList();
            }

            // Only show PUBLISHED courses
            result = result.Where(c => c.status == (int)Status.Published).ToList();

            return result;
        }

        public async Task<LessonsCompletedResponse> GetLessonsCompletedAsync(GetLessonsCompletedRequest request)
        {
            var validate = await ValidateCourseAsync(new ValidateCourseRequest
            {
                CourseId = request.CourseId,
                LessonId = request.LessonId
            });
            if (!validate.Success)
            {
                return new LessonsCompletedResponse
                {
                    Success = false,
                    Message = "Course or Lesson is invalid."
                };
            }

            var currentLesson = await _lessonRepository.GetByIdAsync(request.LessonId);
            var currentModule = await _moduleRepository.GetByIdAsync(currentLesson.moduleId);

            var modules = await _moduleRepository.GetAllAsync(m => m.courseId == request.CourseId);
            var filteredModules = modules.Where(m => !m.IsDeleted && m.order <= currentModule.order);
            List<Guid> completedLessons = new List<Guid>();
            foreach (var module in filteredModules)
            {
                var lessons = await _lessonRepository.GetAllAsync(l => l.moduleId == module.Id);
                var filteredLessons = lessons.Where(l => !l.IsDeleted);
                var orderedLessons = filteredLessons.OrderBy(x => x.orderIndex);
                if (module.order == currentModule.order)
                {
                    foreach (var lesson in orderedLessons)
                    {
                        if (lesson.orderIndex < currentLesson.orderIndex)
                        {
                            completedLessons.Add(lesson.Id);
                        }
                        else if (lesson.orderIndex == currentLesson.orderIndex)
                        {
                            completedLessons.Add(lesson.Id);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var lesson in orderedLessons)
                    {
                        completedLessons.Add(lesson.Id);
                    }
                }
            }
            return new LessonsCompletedResponse
            {
                Success = true,
                Message = "Completed lessons retrieved successfully.",
                completedLessons = completedLessons
            };
        }

        public async Task<CourseReponse> ChangeCourseStatusAsync(ChangeCourseStatusRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null || course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", request.CourseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            var mod = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = request.ModeratorId.ToString() }
            );
            if (!mod.Exists)
            {
                _logger.LogError("Moderator with ID {ModeratorId} does not exist or is not a moderator.", request.ModeratorId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Moderator does not exist or is not a moderator."
                };
            }
            if (course.status == Status.Archived
                 && request.Status == (int)Status.Draft)
            {
                course.status = Status.Draft;
                _courseRepository.Update(course);
                _logger.LogInformation("Course with ID {CourseId} status changed to Draft.", request.CourseId);
            }
            else if (course.status == Status.Draft && request.Status == (int)Status.Archived)
            {
                course.status = Status.Archived;
                _courseRepository.Update(course);
                _logger.LogInformation("Course with ID {CourseId} cannot be changed directly from Draft to Archived.", request.CourseId);
            }
            else
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "You don't have permission to change the course status in this way."
                };
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result < 0)
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "Failed to update status course due to database error"
                };
            }
            else
                return new CourseReponse
                {
                    Success = true,
                    Message = "Update status course successfully.",
                    Course = course
                };
        }

        public async Task<CourseReponse> ModChangeCourseStatus(ChangeCourseStatusRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null || course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", request.CourseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            if (course.status == Status.Published && request.Status == (int)Status.Draft)
            {
                return new CourseReponse
                {
                    Success = false,
                    Message = "Cannot change status from Published to Draft."
                };
            }
            if (request.Status == (int)Status.Archived)
            {
                //gọi hàm handle việc khóa (check tg học xong lâu nhất và gọi batch tự động chạy cũng như mail thông báo khóa)

            }
            else
            {
                course.status = (Status)request.Status;
            }
            course.UpdatedBy = request.ModeratorId;
            _courseRepository.Update(course);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result < 0)
            {
                _logger.LogError("Failed to update status for Course ID {CourseId}.", request.CourseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Failed to update status course due to database error"
                };
            }
            else
            {
                _logger.LogInformation("Course with ID {CourseId} status changed to {Status}.", request.CourseId, request.Status);

                return new CourseReponse
                {
                    Success = true,
                    Message = "Update status course successfully.",
                    Course = course
                };
            }
        }

        public async Task<Response> AutoCheckCourseAsync(AutoCheckCourseRequest request)
        {
            Course course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null || course.IsDeleted)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", request.CourseId);
                return new Response
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var modules = await _moduleRepository.GetAllAsync(m => m.courseId == request.CourseId);
            var filteredModules = modules.Where(m => !m.IsDeleted).ToList();

            //check total module, total lesson có được tính là khóa k phải spam k
            int totalModules = filteredModules.Count;
            int totalLessons = 0;
            foreach (var module in filteredModules)
            {
                var lessons = await _lessonRepository.GetAllAsync(l => l.moduleId == module.Id);
                var filteredLessons = lessons.Where(l => !l.IsDeleted).ToList();
                totalLessons += filteredLessons.Count;
            }
            if (totalModules < 1 || totalLessons < 5)
            {
                return new Response
                {
                    Success = false,
                    Message = "Course is not enable to public."
                };
            }
            return new Response
            {
                Success = true,
                Message = "Auto check completed successfully."
            };
        }

        public async Task<CourseReponse> GetCourseByIdGrpcAsync(Guid courseId)
        {
            var courses = await _courseRepository.FindAsync(c => c.Id == courseId && !c.IsDeleted);
            var course = courses.FirstOrDefault();
            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new CourseReponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }
            if (course.IsDeleted)
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

        public async Task<Response> requestBanCourse(Guid courseId)
        {
            var courses = await _courseRepository.GetAllAsync(c => c.Id == courseId && !c.IsDeleted);
            var course = courses.FirstOrDefault();

            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new Response
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            course.isRequestedBanned = true;
            _courseRepository.Update(course);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result < 0)
            {
                _logger.LogError("Failed to request ban for Course ID {CourseId}.", courseId);
                return new Response
                {
                    Success = false,
                    Message = "Failed to request ban course due to database error"
                };
            }
            else
            {
                _logger.LogInformation("Course with ID {CourseId} has been requested for ban.", courseId);
                return new Response
                {
                    Success = true,
                    Message = "Request ban course successfully."
                };

            }
        }

        public async Task HideCoursesAutomatic()
        {
            // check course có request true và date > last date thì khóa
            var courses = await _courseRepository.GetAllAsync(c => c.isRequestedBanned && !c.IsDeleted && c.status == Status.Published);
            foreach (var course in courses)
            {
                //giả sử khóa sau 1 ngày kể từ ngày học cuối cùng
                var lastDate = await _enrollmentService.GetLastDateCourseAsync(
                    new GetLastDateCoureRequest
                    {
                        CourseId = course.Id.ToString()
                    }
                    );
                DateTime dateTime = DateTime.UtcNow;
                _logger.LogInformation("Current DateTime: {DateTime}", dateTime);
                DateTime lastDateTime = DateTime.Parse(lastDate.LastDate);
                _logger.LogInformation("Last DateTime for Course ID {CourseId}: {LastDateTime}", course.Id, lastDateTime);
                if (lastDateTime.AddDays(1) < dateTime)
                {
                    course.status = Status.Archived;
                }
                _courseRepository.Update(course);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result < 0)
            {
                _logger.LogError("Failed to automatically hide courses.");
            }
            else
            {
                _logger.LogInformation("Automatically hid courses successfully.");
            }
        }
    }
}
