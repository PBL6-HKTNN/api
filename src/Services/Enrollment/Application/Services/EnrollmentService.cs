using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Domain.Entities;
using Codemy.CoursesProto;
using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Domain.Entities;
using Codemy.Enrollment.Domain.Enums;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

using ProtoValidateRequest = Codemy.CoursesProto.GetValidateRequest;

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

        public async Task<EnrollmentResponse> GetCourseWithGrpc(Guid courseId, Guid userId)
        {
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = userId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", userId);
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
                .FindAsync(e => e.courseId == courseId && e.studentId == userId);
            if (existingEnrollment.Count == 0)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User is not enrolled in this course."
                };
            }
            return new EnrollmentResponse
            {
                Success = true,
                Message = "User is enrolled in this course.",
                Enrollment = existingEnrollment.First()
            };
        }
        

        public async Task<EnrollmentResponse> GetCourseAsync(Guid courseId)
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
            if (existingEnrollment.Count == 0)
                {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User is not enrolled in this course."
                };
            }
            return new EnrollmentResponse
            {
                Success = true,
                Message = "User is enrolled in this course.",
                Enrollment = existingEnrollment.First()
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
                var validate = _courseClient.ValidateCourseAsync(new ProtoValidateRequest { CourseId = enrollment.courseId.ToString(), LessonId = request.LessonId.ToString() });
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

        public async Task<EnrollmentResponse> UpdateProgressAsync(UpdateProgressRequest request)
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
                new GetCourseByIdRequest { CourseId = request.CourseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", request.CourseId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var existingEnrollment = await _enrollmentRepository
                .FindAsync(e => e.courseId == request.CourseId && e.studentId == UserId);
            if (existingEnrollment.Count == 0)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User is not enrolled in this course."
                };
            }
            var enrollment = existingEnrollment.First();
            if (enrollment.progressStatus == ProgressStatus.Completed)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User has already completed this course. Don't change progress status."
                };
            }
            //check lessonId thuộc course không
            var validate = _courseClient.ValidateCourseAsync(
                new ProtoValidateRequest { CourseId = request.CourseId.ToString(), LessonId = request.LessonId.ToString() }
            );
            if (!validate.Validate)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Lesson does not belong to this course."
                };
            }
            if (validate.IsLastLesson)
            {
                enrollment.progressStatus = ProgressStatus.Completed;
                enrollment.enrollmentStatus = EnrollmentStatus.Completed;
                enrollment.completionDate = DateTime.UtcNow;
            }
            else
            {
                enrollment.progressStatus = ProgressStatus.InProgress;
            }
            enrollment.lessonId = request.LessonId;
            _enrollmentRepository.Update(enrollment);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to update progress for enrollment {EnrollmentId}.", enrollment.Id);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Failed to update progress."
                };
            }
            return new EnrollmentResponse
            {
                Success = true,
                Enrollment = enrollment
            };
        }

        public async Task<CheckEnrollmentsResponse> CheckEnrollmentsAsync(CheckEnrollmentsRequest request)
        {

            if (request.CourseIds == null || !request.CourseIds.Any())
                return new CheckEnrollmentsResponse { EnrolledCourseIds = new List<string>() };

            var enrolledCourses = await _enrollmentRepository
                .Query()
                .Where(e => e.studentId == request.UserId && request.CourseIds.Contains(e.courseId))
                .Select(e => e.courseId)
                .ToListAsync();

            return new CheckEnrollmentsResponse
            {
                Success = true,
                Message = "Check enrollments completed.",
                EnrolledCourseIds = enrolledCourses.Select(id => id.ToString()).ToList()
            };
        }

        public async Task<EnrollmentResponse> UpdateCurrentView(UpdateCurrentViewRequest request)
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
                new GetCourseByIdRequest { CourseId = request.CourseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", request.CourseId);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var existingEnrollment = await _enrollmentRepository
                .FindAsync(e => e.courseId == request.CourseId && e.studentId == UserId);
            if (existingEnrollment.Count == 0)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "User is not enrolled in this course."
                };
            }
            var enrollment = existingEnrollment.First();
            var validate = _courseClient.ValidateCourseAsync(
                new ProtoValidateRequest { CourseId = request.CourseId.ToString(), LessonId = request.CurrentLessonId.ToString() }
            );
            if (!validate.Validate)
            {
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Lesson does not belong to this course."
                };
            }
            enrollment.currentView = request.CurrentLessonId;
            _enrollmentRepository.Update(enrollment);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to update current view for enrollment {EnrollmentId}.", enrollment.Id);
                return new EnrollmentResponse
                {
                    Success = false,
                    Message = "Failed to update current view."
                };
            }
            return new EnrollmentResponse
            {
                Success = true,
                Enrollment = enrollment
            };
        }

        public async Task<LessonCompletedResponse> GetLessonsCompletedByEnrollmentIdAsync(Guid enrollmentId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new LessonCompletedResponse
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
                return new LessonCompletedResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }
            var existingEnrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (existingEnrollment == null)
            {
                return new LessonCompletedResponse
                {
                    Success = false,
                    Message = "User is not enrolled in this course."
                };
            }
            var lessonsCompleted = await _courseClient.GetLessonsCompletedAsync(
                new GetValidateRequest
                {
                    CourseId = existingEnrollment.courseId.ToString(),
                    LessonId = existingEnrollment.lessonId?.ToString() ?? ""
                }
            );
            if (!lessonsCompleted.Success)
            {
                return new LessonCompletedResponse
                {
                    Success = false,
                    Message = "Failed to retrieve completed lessons."
                };
            }
            return new LessonCompletedResponse
            {
                Success = true,
                CompletedLessonIds = lessonsCompleted.CompletedLessons.Select(id => Guid.Parse(id)).ToList()
            };
        }
    }
}
