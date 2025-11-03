using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sprache;
using System.Security.Claims;

namespace Codemy.Courses.Application.Services
{
    internal class ModuleService : IModuleService
    {
        private readonly ILogger<ModuleService> _logger;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<Module> _moduleRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ModuleService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<ModuleService> logger,
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
        public async Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new ModuleResponse
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
                return new ModuleResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var course = await _courseRepository.GetByIdAsync(request.courseId);
            if (course == null)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", request.courseId);
                return new ModuleResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            if (userId != course.instructorId)
            {
                _logger.LogError("User with ID {UserId} is not authorized to create module for course ID {CourseId}.", userId, request.courseId);
                return new ModuleResponse
                {
                    Success = false,
                    Message = "User is not authorized to create module for this course."
                };
            }

            var existingModules = await _moduleRepository.FindAsync(m => m.courseId == request.courseId && m.order == request.order);
            if (existingModules.Any())
            {
                _logger.LogError("Module with order {Order} already exists for course ID {CourseId}.", request.order, request.courseId);
                return new ModuleResponse
                {
                    Success = false,
                    Message = "Module with the same order already exists for this course."
                };
            }
            var module = new Module
            {
                Id = Guid.NewGuid(),
                title = request.title,
                order = request.order,
                courseId = request.courseId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedBy = userId
            };
            await _moduleRepository.AddAsync(module);
            course.numberOfModules += 1;
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new ModuleResponse
                {
                    Success = false,
                    Message = "Failed to create module."
                };
            }
            return new ModuleResponse
            {
                Success = true,
                Message = "Module created successfully.",
                Module = module
            };
        }

        public async Task<ModuleListResponse> GetModules()
        {
            try
            {
                var module = await _moduleRepository.GetAllAsync();
                var filteredModules = module.Where(m => !m.IsDeleted);
                return new ModuleListResponse
                {
                    Success = true,
                    Message = "Get list module successfully.",
                    Modules = filteredModules.ToList()
                };
            }
            catch (Exception e)
            {
                return new ModuleListResponse
                {
                    Success = false,
                    Message = e.Message,
                    Modules = new List<Module>()
                };
            }
        }

        public async Task<LessonListResponse> GetLessonByModuleId(Guid moduleId)
        {
            try
            {
                var module = await _moduleRepository.GetByIdAsync(moduleId);
                if (module == null)
                {
                    _logger.LogError("Module with ID {ModuleId} does not exist.", moduleId);
                    return new LessonListResponse
                    {
                        Success = false,
                        Message = "Module does not exist."
                    };
                }
                if (module.IsDeleted)
                {
                    _logger.LogError("Module with ID {ModuleId} is deleted.", moduleId);
                    return new LessonListResponse
                    {
                        Success = false,
                        Message = "Module is deleted."
                    };
                }
                var lessons = await _lessonRepository.FindAsync(m => m.moduleId == moduleId);
                if (lessons.ToList().Count == 0)
                {
                    return new LessonListResponse
                    {
                        Success = false,
                        Message = "No lessons available in this module"
                    };
                }
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

        public async Task<ModuleResponse> GetModuleById(Guid moduleId)
        {
            try
            {
                var result = await _moduleRepository.GetByIdAsync(moduleId);
                if (result == null)
                {
                    _logger.LogError("Module with ID {ModuleId} does not exist.", moduleId);
                    return new ModuleResponse
                    {
                        Success = false,
                        Message = "Module does not exist."
                    };
                }
                if (result.IsDeleted)
                {
                    _logger.LogError("Module with ID {ModuleId} is deleted.", moduleId);
                    return new ModuleResponse   
                    {
                        Success = false,
                        Message = "Module is deleted."
                    };
                }
                return new ModuleResponse
                {
                    Success = true,
                    Message = "Module retrieved successfully.",
                    Module = result
                };
            }
            catch (Exception e)
            {
                return new ModuleResponse
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }

        public async Task<ModuleResponse> DeleteModuleAsync(Guid moduleId)
        {
            var result = await _moduleRepository.GetByIdAsync(moduleId);
            if (result == null)
            {
                return new ModuleResponse
                {
                    Success = false,
                    Message = "Module not found."
                };
            }
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new ModuleResponse
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
                _logger.LogError("User with ID {UserId} is not authorized to delete module ID {ModuleId}.", userId, moduleId);
                return new ModuleResponse
                {
                    Success = false,
                    Message = "User is not authorized to delete this module."
                };
            }
            _moduleRepository.Delete(result);
            var course = await _courseRepository.GetByIdAsync(result.courseId);
            course.numberOfModules -= 1;
            var listLessonAfterDelete = await _lessonRepository
                .FindAsync(l => l.moduleId == result.Id);
            foreach (var lesson in listLessonAfterDelete)
            {
                _lessonRepository.Delete(lesson);
            }
            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult <= 0)
            {
                return new ModuleResponse
                {
                    Success = false,
                    Message = "Failed to delete module due to a database error."
                };
            }
            return new ModuleResponse
            {
                Success = true,
                Message = "Module deleted successfully.", 
            };
        }

        public async Task<ModuleResponse> UpdateModuleAsync(Guid moduleId, CreateModuleRequest request)
        {
            var result = await _moduleRepository.GetByIdAsync(moduleId);
            if (result == null)
            {
                return new ModuleResponse
                {
                    Success = false,
                    Message = "Module not found."
                };
            }

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new ModuleResponse
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
                _logger.LogError("User with ID {UserId} is not authorized to update module ID {LessonId}.", userId, moduleId);
                return new ModuleResponse
                {
                    Success = false,
                    Message = "User is not authorized to update this module."
                };
            }
            var existingModule = await _moduleRepository
                .FindAsync(l => l.courseId == request.courseId && l.order == request.order && l.Id != moduleId);
            if (existingModule.Any())
            {
                Console.WriteLine("Duplicate module IDs:");
                foreach (var l in existingModule)
                {
                    Console.WriteLine(l.Id);
                }

                _logger.LogError(
                    "Module with order index {OrderIndex} already exists for course ID {CourseId}. Duplicate IDs: {Ids}",
                    request.order,
                    request.courseId,
                    string.Join(", ", existingModule.Select(l => l.Id))
                );

                return new ModuleResponse
                {
                    Success = false,
                    Message = "Module with the same order index already exists in this course."
                };
            }

            result.title = request.title; 
            result.order = request.order;
            result.UpdatedAt = DateTime.UtcNow;
            _moduleRepository.Update(result);
            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (saveResult <= 0)
            {
                return new ModuleResponse
                {
                    Success = false,
                    Message = "Failed to update module due to a database error."
                };
            }
            return new ModuleResponse
            {
                Success = true,
                Message = "Module updated successfully.",
                Module = result
            };
        }
    }
}
