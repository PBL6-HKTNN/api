using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Notification.Application.DTOs;
using Codemy.Notification.Application.Interfaces;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json.Linq;
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
      _logger = logger;
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
                        <p><a href='{_client_url}/auth/verify?token={Token}&email={To}'>Verify Email</a></p>
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

        public async Task InformRequestResolved(EmailInformRequestContent content)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(content.From));
            email.To.Add(MailboxAddress.Parse(content.To));
            email.Subject = "Request Update Notification";

            var body = $@"
            <html>
              <body style='font-family:Segoe UI, sans-serif;'>
                <h2>Request Update</h2>

                <p>Hello,</p>

                <p>Your request has been processed. Below are the details:</p>

                <table style='border-collapse:collapse; font-size:14px;'>
                  <tr>
                    <td style='padding:6px 12px; font-weight:bold;'>Request ID:</td>
                    <td style='padding:6px 12px;'>{content.requestId}</td>
                  </tr>
                  <tr>
                    <td style='padding:6px 12px; font-weight:bold;'>Request Type:</td>
                    <td style='padding:6px 12px;'>{content.RequestType}</td>
                  </tr>
                  <tr>
                    <td style='padding:6px 12px; font-weight:bold;'>Description:</td>
                    <td style='padding:6px 12px;'>{content.Description}</td>
                  </tr>
                  <tr>
                    <td style='padding:6px 12px; font-weight:bold;'>Status:</td>
                    <td style='padding:6px 12px; color:#007bff;'>{content.Status}</td>
                  </tr>
                  {(content.Response != null ? $@"
                  <tr>
                    <td style='padding:6px 12px; font-weight:bold;'>Response:</td>
                    <td style='padding:6px 12px;'>{content.Response}</td>
                  </tr>" : "")}
                  {(content.CourseId != null ? $@"
                  <tr>
                    <td style='padding:6px 12px; font-weight:bold;'>Course ID:</td>
                    <td style='padding:6px 12px;'>{content.CourseId}</td>
                  </tr>" : "")}
                </table>

                <p>If you have any questions, feel free to contact us.</p>

                <br/>

                <p>From: <strong>{content.From}</strong></p>
                <p>Thank you,<br/><strong>Codemy Team</strong></p>
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
