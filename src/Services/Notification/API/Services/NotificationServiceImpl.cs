using Codemy.NotificationProto;
using Codemy.Notification.Application.Interfaces;
using Grpc.Core;

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
    }
}
