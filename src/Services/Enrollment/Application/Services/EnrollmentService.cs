using Codemy.BuildingBlocks.Core;
using Codemy.CoursesProto;
using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Domain.Entities;
using Codemy.Enrollment.Domain.Enums;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Codemy.Enrollment.Application.Services
{
    internal class EnrollmentService : IEnrollmentService
    {
        private readonly ILogger<EnrollmentService> _logger;
        private readonly IRepository<Enrollments> _enrollmentRepository;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CoursesService.CoursesServiceClient _courseClient;

        public EnrollmentService(
            ILogger<EnrollmentService> logger,
            IRepository<Enrollments> enrollmentRepository,
            IdentityService.IdentityServiceClient client,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            CoursesService.CoursesServiceClient courseClient)
        {
            _logger = logger;
            _enrollmentRepository = enrollmentRepository;
            _client = client;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _courseClient = courseClient;
        }

        public async Task<EnrollmentResponse> EnrollInCourseAsync(Guid courseId, Guid UserId)
        {
            var courseExists = await _courseClient.GetCourseByIdAsync(
                new GetCourseByIdRequest { CourseId = courseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var existingEnrollment = await _enrollmentRepository
                .FindAsync(e => e.courseId == courseId && e.studentId == UserId);
            if (existingEnrollment.Count != 0)
                { 
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User is already enrolled in this course."
                };
            }

            Enrollments enrollments = new Enrollments
            {
                Id = Guid.NewGuid(),
                courseId = courseId,
                studentId = UserId,
                progressStatus = ProgressStatus.NotStarted,
                enrollmentStatus = EnrollmentStatus.Active,
                enrollmentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = UserId
            };
            await _enrollmentRepository.AddAsync(enrollments);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to enroll user {UserId} in course {CourseId}.", UserId, courseId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Failed to enroll in course."
                };
            }
            return new EnrollmentResponse
            {
                Success = true,
                Enrollment = enrollments
            };
        }

        public async Task<EnrollmentResponse> EnrollInCourseAsyncWithoutGrpc(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }
            var courseExists = await _courseClient.GetCourseByIdAsync(
                new GetCourseByIdRequest { CourseId = courseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var existingEnrollment = await _enrollmentRepository
                .FindAsync(e => e.courseId == courseId && e.studentId == UserId);
            if (existingEnrollment.Count != 0)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User is already enrolled in this course."
                };
            }

            Enrollments enrollments = new Enrollments
            {
                Id = Guid.NewGuid(),
                courseId = courseId,
                studentId = UserId,
                progressStatus = ProgressStatus.NotStarted,
                enrollmentStatus = EnrollmentStatus.Active,
                enrollmentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = UserId
            };
            await _enrollmentRepository.AddAsync(enrollments);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to enroll user {UserId} in course {CourseId}.", UserId, courseId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Failed to enroll in course."
                };
            }
            return new EnrollmentResponse
            {
                Success = true,
                Enrollment = enrollments
            };
        }

        public async Task<Response> GetCourseAsync(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new Response
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new Response
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }
            var courseExists = await _courseClient.GetCourseByIdAsync(
                new GetCourseByIdRequest { CourseId = courseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new Response
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var existingEnrollment = await _enrollmentRepository
                .FindAsync(e => e.courseId == courseId && e.studentId == UserId);
            if (existingEnrollment.Count == 0)
                {
                return new Response
                {
                    Success = false,
                    Message = "User is not enrolled in this course."
                };
            }
            return new Response
            {
                Success = true,
            };
        }

        public async Task<EnrollmentResponse> UpdateEnrollmentStatusAsync(UpdateEnrollmentRequest request)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId);

            if (enrollment == null)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Enrollment not found."
                };
            }
            if (request.ProgressStatus.HasValue)
            {
                enrollment.progressStatus = request.ProgressStatus.Value;
            }
            if (request.LessonId.HasValue)
            {
                var validate = _courseClient.ValidateCourseAsync(new GetValidateRequest { CourseId = enrollment.courseId.ToString(), LessonId = request.LessonId.ToString() });
                if (!validate.Validate)
                {
                    return new EnrollmentResponse
                    {
                        Success = false,
                        Message = "Lesson does not belong to this course."
                    };
                }
                enrollment.lessonId = request.LessonId.Value;

            }
            if (request.CompletionDate.HasValue)
            {
                if (enrollment.enrollmentDate > request.CompletionDate.Value)
                {
                    return new EnrollmentResponse
                    {
                        Success = false,
                        Message = "Completion date cannot be earlier than enrollment date."
                    };
                }
                enrollment.completionDate = request.CompletionDate.Value;
            }
            if (request.CertificateUrl != null)
            {
                enrollment.certificateUrl = request.CertificateUrl;
            }
            if (request.CertificateExpiryDate.HasValue)
            {
                enrollment.certificateExpiryDate = request.CertificateExpiryDate.Value;
            }
            _enrollmentRepository.Update(enrollment);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to update enrollment {EnrollmentId}.", request.EnrollmentId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Failed to update enrollment status."
                };
            }
            return new EnrollmentResponse
            {
                Success = true,
                Enrollment = enrollment
            };
        }

        public async Task<CoursesResponse> GetMyCoursesAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            if (userId == Guid.Empty)
            {
                return new CoursesResponse
                {
                    Success = false,
                    Message = "Invalid user ID."
                };
            }

            var enrollments = await _enrollmentRepository
                .FindAsync(e => e.studentId == userId);

            var courseIds = enrollments.Select(e => e.courseId).ToList();
            if (!courseIds.Any())
            {
                return new CoursesResponse
                {
                    Success = true,
                    Courses = new List<CourseDto>()
                };
            }

            var pagedCourseIds = courseIds
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var courses = new List<CourseDto>();

            foreach (var courseId in pagedCourseIds)
            {
                try
                {
                    var courseResponse = await _courseClient.GetCourseByIdAsync(
                        new GetCourseByIdRequest { CourseId = courseId.ToString() }
                    );

                    if (courseResponse.Exists && courseResponse.CourseId != null)
                    {
                        courses.Add(new CourseDto
                        {
                            Id = Guid.Parse(courseResponse.CourseId),
                            InstructorId = Guid.Parse(courseResponse.InstructorId),
                            Title = courseResponse.Title,
                            Thumbnail = courseResponse.Thumbnail,
                            Description = courseResponse.Description,
                            Price = decimal.TryParse(courseResponse.Price, out var price) ? price : 0
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error fetching course {CourseId}", courseId);
                }
            }

            return new CoursesResponse
            {
                Success = true,
                Courses = courses
            };
        }

    }
}
