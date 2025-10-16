using Codemy.Notification.Application.DTOs;

namespace Codemy.Notification.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
