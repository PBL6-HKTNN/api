using Codemy.NotificationProto;

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
    }
}
