using Codemy.BuildingBlocks.Core;
using Codemy.CoursesProto;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using DotNetEnv;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;

namespace Codemy.Enrollment.Application.Services
{
    internal class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly ILogger<GoogleCalendarService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CoursesService.CoursesServiceClient _courseClient;

        public GoogleCalendarService(
            ILogger<GoogleCalendarService> logger,
            IHttpContextAccessor httpContextAccessor,
            CoursesService.CoursesServiceClient courseClient)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _courseClient = courseClient;
            Env.Load();
        }

        public async Task<CalendarResponse> AddCourseToCalendarAsync(Guid courseId)
        {
            var course = await _courseClient.GetCourseByIdAsync(new GetCourseByIdRequest { CourseId = courseId.ToString() });
            if (course == null)
            {
                _logger.LogError("Course not found with ID: {CourseId}", courseId);
                throw new Exception("Course not found");
            }
            // 2️⃣ Get Refresh Token from Identity
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                _logger.LogError("User not authenticated or token missing.");
                throw new Exception("User not authenticated or token missing.");
            }

            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);

            var refreshToken = string.Empty;
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("User has not connected Google Calendar");
                return new CalendarResponse
                {
                    Success = false,
                    Message = "User has not connected Google Calendar"
                };
            }

            // 3️⃣ Build Google Credential
            var token = new TokenResponse { RefreshToken = refreshToken };

            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");

            var credential = new UserCredential(
                new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            ClientId = clientId,
                            ClientSecret = _config["Google:ClientSecret"]
                        },
                        Scopes = new[] { CalendarService.Scope.Calendar }
                    }),
                userId.ToString(),
                token
            );

            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Codemy Scheduler"
            });

            // 4️⃣ Create 3 events per week (Mon, Wed, Fri)
            var today = DateTime.Today;
            var sessionDates = new[]
            {
            today.AddDays((int)DayOfWeek.Monday - (int)today.DayOfWeek),
            today.AddDays((int)DayOfWeek.Wednesday - (int)today.DayOfWeek),
            today.AddDays((int)DayOfWeek.Friday - (int)today.DayOfWeek),
        };

            foreach (var date in sessionDates)
            {
                var start = date.AddHours(19); // 7:00 PM
                var end = start.AddHours(1);   // 1 hour session

                var newEvent = new Event
                {
                    Summary = $"Study: {course.Title}",
                    Description = course.Description,
                    Start = new EventDateTime
                    {
                        DateTime = start,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    End = new EventDateTime
                    {
                        DateTime = end,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    }
                };

                await service.Events.Insert(newEvent, "primary").ExecuteAsync();
            }

            return true;

        }
    }
}
