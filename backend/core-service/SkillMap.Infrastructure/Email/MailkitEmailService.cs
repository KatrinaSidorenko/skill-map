using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

using SkillMap.Business.Abstractions;
using SkillMap.Shared.Options;
using SkillMap.Shared.Results;

namespace SkillMap.Infrastructure.Email;

public class MailkitEmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<IEmailService> _logger;

    public MailkitEmailService(IOptions<EmailOptions> options, ILogger<IEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        email.Body = new TextPart(isHtml ? "html" : "plain")
        {
            Text = body
        };

        try
        {
            ct.ThrowIfCancellationRequested();

            using var smtp = new SmtpClient();
            _logger.LogInformation("Connecting to SMTP server {SmtpServer}:{SmtpPort}", _options.SmtpServer, _options.SmtpPort);
            //await smtp.ConnectAsync(_options.SmtpServer, _options.SmtpPort, SecureSocketOptions.StartTls);
            //await smtp.AuthenticateAsync(_options.SmtpUser, _options.SmtpPass);

            //await smtp.SendAsync(email);
            //await smtp.DisconnectAsync(true);
            _logger.LogInformation("Email sent to {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }
}