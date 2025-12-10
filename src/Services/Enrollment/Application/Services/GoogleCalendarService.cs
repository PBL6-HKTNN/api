using Codemy.BuildingBlocks.Core;
using Codemy.CoursesProto;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.IdentityProto;
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
        private readonly IdentityService.IdentityServiceClient _identityClient;

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
            // Get Refresh Token from Identity
            var userToken = _httpContextAccessor.HttpContext?.User;
            if (userToken == null || !userToken.Identity?.IsAuthenticated == true)
            {
                _logger.LogError("User not authenticated or token missing.");
                throw new Exception("User not authenticated or token missing.");
            }

            var userIdClaim = userToken.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                           ?? userToken.FindFirst("sub")?.Value
                           ?? userToken.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            var user = await _identityClient.GetUserByIdAsync(new GetUserByIdRequest { UserId = userId.ToString() });
            if (user == null) {
                _logger.LogError("User not found with ID: {UserId}", userId);
                throw new Exception("User not found");
            }


            var refreshToken = user.RefreshToken;
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
            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
            var credential = new UserCredential(
                new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret
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

            var links = new List<string>();

            foreach (var date in sessionDates)
            {
                var start = date.AddHours(19);
                var end = start.AddHours(1);

                var newEvent = new Event
                {
                    Summary = $"Study: {course.Title}",
                    Description = course.Description,
                    Start = new EventDateTime { DateTime = start, TimeZone = "Asia/Ho_Chi_Minh" },
                    End = new EventDateTime { DateTime = end, TimeZone = "Asia/Ho_Chi_Minh" }
                };

                var createdEvent = await service.Events.Insert(newEvent, "primary").ExecuteAsync();

                // 🔥 EVENT LINK HERE
                links.Add(createdEvent.HtmlLink);
            }


            return new CalendarResponse
            {
                Success = true,
                Message = "Course sessions added to Google Calendar successfully.",
                CalendarLink = links
            };

        }
    }
}
