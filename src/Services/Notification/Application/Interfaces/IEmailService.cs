using Codemy.Notification.Application.DTOs;

namespace Codemy.Notification.Application.Interfaces
{
    public interface IEmailService
    {
        Task InformRequestResolved(EmailInformRequestContent content);
        Task SendEmailAsync(string From, string To, string Token);
        Task SendResetPasswordToken(string From, string To, string Token);
    }
}
