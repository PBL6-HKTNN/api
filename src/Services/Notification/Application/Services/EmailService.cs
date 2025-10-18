using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Notification.Application.Interfaces;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
namespace Codemy.Notification.Application.Services
{
  internal class EmailService : IEmailService
  {
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtp_host;
    private readonly string _smtp_user;
    private readonly string _smtp_password;
    private readonly int _smtp_port;
    private readonly string _client_url;
    public EmailService(ILogger<EmailService> logger)
    {
      LogExtensions.LoadEnvFile(_logger);

      _smtp_host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? throw new ArgumentException("SMTP HOST not configured");
      _smtp_user = Environment.GetEnvironmentVariable("SMTP_USER") ?? throw new ArgumentException("SMTP USER not configured");
      _smtp_password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new ArgumentException("SMTP PASSWORD not configured");
      _smtp_port = Int32.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")!);
      _client_url = Environment.GetEnvironmentVariable("CLIENT_BASE_URL") ?? "";
    }
    public async Task SendResetPasswordToken(string From, string To, string Token)
    {
      var email = new MimeMessage();
      email.From.Add(MailboxAddress.Parse(From));
      email.To.Add(MailboxAddress.Parse(To));

      email.Subject = "Reset Password";
      var body = $@"
                <html>
                  <body style='font-family:Segoe UI, sans-serif;'>
                    <h2>Reset Your Password</h2>
                    <p>Hello,</p>
                    <p>Use the following OTP to reset your password:</p>
                    <h3 style='color:#007bff;'>{Token}</h3>
                    <p>If you did not request a password reset, please ignore this email.</p>
                    <p>From: <strong>Codemy</strong></p>
                  </body>
                </html>";
      email.Body = new TextPart("html") { Text = body };

      using var smtp = new MailKit.Net.Smtp.SmtpClient();
      await smtp.ConnectAsync(_smtp_host, _smtp_port, MailKit.Security.SecureSocketOptions.StartTls);
      await smtp.AuthenticateAsync(_smtp_user, _smtp_password);
      await smtp.SendAsync(email);
      await smtp.DisconnectAsync(true);
    }
    public async Task SendEmailAsync(string From, string To, string Token)
    {
      var email = new MimeMessage();
      email.From.Add(MailboxAddress.Parse(From));
      email.To.Add(MailboxAddress.Parse(To));
      email.Subject = "Verify email";
      var body = $@"
                    <html>
                      <body style='font-family:Segoe UI, sans-serif;'>
                        <h2>Verify Your Email</h2>
                        <p>Hello,</p>
                        <p>Use the following OTP to verify your email address:</p>
                        <h3 style='color:#007bff;'>{Token}</h3>
                        <p>Or click the link below to verify your email:</p>
                        <p><a href='{_client_url}/auth/verify?token={Token}'>Verify Email</a></p>
                        <p>From: <strong>Codemy</strong></p>
                      </body>
                    </html>";

      email.Body = new TextPart("html") { Text = body };

      using var smtp = new MailKit.Net.Smtp.SmtpClient();
      await smtp.ConnectAsync(_smtp_host, _smtp_port, MailKit.Security.SecureSocketOptions.StartTls);
      await smtp.AuthenticateAsync(_smtp_user, _smtp_password);
      await smtp.SendAsync(email);
      await smtp.DisconnectAsync(true);
    }

  }
}
