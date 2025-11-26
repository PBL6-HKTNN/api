using Codemy.EnrollmentsProto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Payment.Application.Services
{
    internal class PaymentGrpcEnrollmentService
    {
        private readonly EnrollmentService.EnrollmentServiceClient _enrollmentClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PaymentGrpcEnrollmentService(
            EnrollmentService.EnrollmentServiceClient enrollmentClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _enrollmentClient = enrollmentClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateEnrollmentResponse> CreateEnrollmentAsync(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                throw new Exception("User not authenticated or token missing.");
            }
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;
            var UserId = Guid.Parse(userIdClaim);
            var enrollmentRequest = new CreateEnrollmentRequest
            {
                CourseId = courseId.ToString(),
                UserId = UserId.ToString()
            };
            var enrollmentResponse = await _enrollmentClient.CreateEnrollmentAsync(enrollmentRequest);
            return enrollmentResponse;
        }
        public async Task<CreateEnrollmentResponse> CreateEnrollmentAsync(Guid courseId, Guid userId)
        {
            var enrollmentRequest = new CreateEnrollmentRequest
            {
                CourseId = courseId.ToString(),
                UserId = userId.ToString()
            };
            var enrollmentResponse = await _enrollmentClient.CreateEnrollmentAsync(enrollmentRequest);
            return enrollmentResponse;
        }
    }
}
