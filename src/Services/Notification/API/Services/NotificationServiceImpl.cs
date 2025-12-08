using Codemy.NotificationProto;
using Codemy.Notification.Application.Interfaces;
using Grpc.Core;
using Codemy.Notification.Application.DTOs;

namespace Codemy.Notification.API.Services
{
    public class NotificationServiceImpl : NotificationService.NotificationServiceBase
    {
        private readonly IEmailService _emailService;

        public NotificationServiceImpl(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public override async Task<SendEmailResponse> SendEmail(SendEmailRequest request, ServerCallContext context)
        { 
            await _emailService.SendEmailAsync(request.From, request.To, request.Token);

            return new SendEmailResponse { Success = true }; 
        }

        public override async Task<SendEmailResponse> SendResetPasswordToken(SendEmailRequest request, ServerCallContext context)
        {
            await _emailService.SendResetPasswordToken(request.From, request.To, request.Token);
            return new SendEmailResponse { Success = true };
        }

        public override async Task<SendEmailResponse> InformHideCourse(SendEmailInformHideCourseRequest request, ServerCallContext context)
        {
            var content = new InformHideCourseRequest
            {
                From = request.From,
                To = request.To,
                CourseId = Guid.Parse(request.CourseId),
                Description = request.Description,
                DateTime = request.Datetime,
                CourseTitle = request.CourseTitle,
            };
            await _emailService.InformHideCourse(content);
            return new SendEmailResponse { Success = true };
        }

        public override async Task<SendEmailResponse> InformRequestResolved(SendEmailResultRequest request, ServerCallContext context)
        {
            var content = new EmailInformRequestContent
            {
                From = request.From,
                To = request.To,
                RequestId = Guid.Parse(request.RequestId),
                RequestType = request.RequestType,
                Description = request.Description,
                Status = request.Status,
            };
            if (!string.IsNullOrEmpty(request.Response))
            {
                content.Response = request.Response;
            }
            if (!string.IsNullOrEmpty(request.CourseId))
            {
                content.CourseId = Guid.Parse(request.CourseId);
            }
            if (!string.IsNullOrEmpty(request.ReviewId))
            {
                content.ReviewId = Guid.Parse(request.ReviewId);
            }
            await _emailService.InformRequestResolved(content);
            return new SendEmailResponse { Success = true };
        }
    }
}
