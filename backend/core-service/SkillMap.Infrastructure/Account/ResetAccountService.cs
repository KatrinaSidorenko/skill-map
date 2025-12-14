using LearningPlatform.Shared.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using SkillMap.Business.Abstractions;
using SkillMap.Infrastructure.Email;
using SkillMap.Shared.Results;
using System.Security.Cryptography;
using System.Text;

namespace SkillMap.Infrastructure.Account;

// todo: add rate limiting to prevent abuse
public class ResetAccountService(IEmailService emailService, ICacheService cacheService, ILogger<IResetAccountService> logger) : IResetAccountService
{
    private const int TokenExpirationMinutes = 10;
    private const string CacheKeyPrefix = "reset_token";
    public static string GenerateSecureToken(int byteLength = 64, bool urlSafe = true)
    {
        var tokenData = new byte[byteLength];
        RandomNumberGenerator.Fill(tokenData);

        return GetSafeToken(Convert.ToBase64String(tokenData));
    }

    public static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashedBytes = sha256.ComputeHash(tokenBytes);
        
        return GetSafeToken(Convert.ToBase64String(hashedBytes));
    }

    private static string GetSafeToken(string token)
    {
        return token.Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
    }

    public async Task<Result<bool>> ResetPassword(string email, CancellationToken ct)
    {
        var token = GenerateSecureToken();
        var hashedToken = HashToken(token);
        
        logger.LogWarning("token: {Token}, hashedToken: {HashedToken}", token, hashedToken);
        var key = $"{CacheKeyPrefix}:{hashedToken}";
        await cacheService.SetAsync(key, email, TimeSpan.FromMinutes(TokenExpirationMinutes), ct);

        // todo: Use a proper email template
        var subject = "Password Reset Request";
        var body = $"Use the following token to reset your password. This token is valid for {TokenExpirationMinutes} minutes.\n\nToken: {token}";
        var sendResult = await emailService.SendEmailAsync(email, subject, body, ct: ct);
        if (!sendResult.IsSuccessful)
        {
            return ResultType.FailedToSendEmail<bool>(email);
        }


        return Result.Success(true);
    }

    public async Task<Result<bool>> VerifyResetToken(string token, CancellationToken ct)
    {
        var hashedToken = HashToken(token);
        var key = $"{CacheKeyPrefix}:{hashedToken}";
        logger.LogWarning("Verifying token. Key: {Key}", key);
        var cachedResult = await cacheService.TryGetAsync<string>(key, ct);
        if (!cachedResult.found || cachedResult.value is null)
        {
            return ResultType.InvalidOrExpiredToken<bool>();
        }

        return Result.Success(true);
    }

    public async Task<Result<string>> GetEmailByToken(string token, CancellationToken ct)
    {
        var hashedToken = HashToken(token);
        var key = $"{CacheKeyPrefix}:{hashedToken}";
        var cachedResult = await cacheService.TryGetAsync<string>(key, ct);
        if (!cachedResult.found || cachedResult.value is null)
        {
            return ResultType.InvalidOrExpiredToken<string>();
        }
        return Result.Success(cachedResult.value);
    }
}
