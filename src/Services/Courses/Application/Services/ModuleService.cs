using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Identity.Domain.Entities;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
                return new ModuleListResponse
                {
                    Success = true,
                    Message = "Get list module successfully.",
                    Modules = module.ToList()
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
    }
}
