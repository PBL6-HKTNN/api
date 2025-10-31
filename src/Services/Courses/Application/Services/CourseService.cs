using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.IdentityProto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Codemy.Courses.Application.Services
{
    internal class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IdentityService.IdentityServiceClient _client;
        public CourseService(
            ILogger<CourseService> logger,
            IRepository<Course> courseRepository,
            IRepository<Category> categoryRepository,
            IUnitOfWork unitOfWork,
            IdentityService.IdentityServiceClient client,
            IRepository<Enrollment> enrollmentRepository)
        {
            _logger = logger;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _enrollmentRepository = enrollmentRepository;
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

        public async Task<IEnumerable<MyCourseDto>> GetMyCoursesAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var query = _enrollmentRepository
                .Query()
                .Include(e => e.Course) 
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrolledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new MyCourseDto
                {
                    Id = e.Course.Id,
                    Title = e.Course.title,
                    Thumbnail = e.Course.thumbnail,
                    Level = e.Course.level.ToString(),
                    Price = e.Course.price,
                    AverageRating = e.Course.averageRating,
                    Language = e.Course.language,
                    Progress = e.Progress.ToString(),
                    Status = e.Status.ToString(),
                    EnrolledAt = e.EnrolledAt
                });

            return await query.ToListAsync();
        }
    }
}
