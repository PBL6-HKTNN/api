using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Courses.Domain.Enums;
using Codemy.Identity.Domain.Entities;
using Codemy.IdentityProto;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Codemy.Courses.Application.Services
{
    internal class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IdentityService.IdentityServiceClient _client;
        public CourseService(
            ILogger<CourseService> logger,
            IRepository<Course> courseRepository,
            IRepository<Category> categoryRepository,
            IUnitOfWork unitOfWork,
            IdentityService.IdentityServiceClient client)
        {
            _logger = logger;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _client = client;
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
            return new CourseReponse
            {
                Success = true,
                Message = "Course created successfully.",
                Course = course
            };
        }

        public Task<CourseReponse> GetCourseByIdAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesAsync(
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
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.title,
                    Description = c.description,
                    Thumbnail = c.thumbnail,
                    Price = c.price,
                    Language = c.language,
                    Level = c.level.ToString(),
                    AverageRating = c.averageRating,
                    NumberOfReviews = c.numberOfReviews
                })
                .ToListAsync();

            return courses;
        }
    }
}
