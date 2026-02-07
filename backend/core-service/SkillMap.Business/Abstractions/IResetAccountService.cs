using SkillMap.Shared.Results;

namespace SkillMap.Business.Abstractions;

public interface IResetAccountService
{
    Task<Result<string>> GetEmailByToken(string token, CancellationToken ct);
    Task<Result<bool>> ResetPassword(string email, CancellationToken ct);
    Task<Result<bool>> VerifyResetToken(string token, CancellationToken ct);
}