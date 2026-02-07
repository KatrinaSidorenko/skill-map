using SkillMap.Shared.Results;

namespace SkillMap.Infrastructure.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default);
}