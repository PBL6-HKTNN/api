using Codemy.NotificationProto;
using Newtonsoft.Json.Linq;

namespace Codemy.Identity.Application.Services
{
    public class EmailSender
    {
        private readonly NotificationService.NotificationServiceClient _client;

        public EmailSender(NotificationService.NotificationServiceClient client)
        {
            _client = client;
        }

        public async Task SendAsync(string from, string to, string token)
        {
            await _client.SendEmailAsync(new SendEmailRequest
            {
                From = from,
                To = to,
                Token = token
            });
        }

        public async Task SendResetPasswordToken(string from, string to, string token)
        {
            await _client.SendResetPasswordTokenAsync(new SendEmailRequest
            {
                From = from,
                To = to,
                Token = token
            });
        }

        public async Task InformRequestResolved(string from, string to, Guid requestId, string requestType, string description, string status, string? response = null, Guid? courseId = null, Guid? reviewId = null)
        {
            var request = new SendEmailResultRequest
            {
                From = from,
                To = to,
                RequestId = requestId.ToString(),
                RequestType = requestType,
                Description = description,
                Status = status
            };
            if (response != null)
            {
                request.Response = response;
            }
            if (courseId.HasValue)
            {
                request.CourseId = courseId.Value.ToString();
            }
            if (reviewId.HasValue)
            {
                request.ReviewId = reviewId.Value.ToString();
            }
            await _client.InformRequestResolvedAsync(request);
        }

        public async Task InformHideCourse(string from, string to, Guid courseId, string description, string dateTime, string courseTitle)
        {
            await _client.InformHideCourseAsync(new SendEmailInformHideCourseRequest
            {
                From = from,
                To = to,
                CourseId = courseId.ToString(),
                Description = description,
                Datetime = dateTime,
                CourseTitle = courseTitle
            });
        }
    }
}
