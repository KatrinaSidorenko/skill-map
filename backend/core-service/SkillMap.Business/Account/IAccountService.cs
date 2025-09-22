using SkillMap.Business.Account.Models;
using SkillMap.Core.Entities;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Account;

public interface IAccountService
{
    Task<Result<UserDto>> Login(LoginDto loginDto, CancellationToken ct);
    Task<Result<bool>> Register(UserRegistrationDto userDto, CancellationToken ct);
    Task<Result<object>> ResetPassword(string email, CancellationToken ct);
    Task<Result<object>> SetNewPassword(SetNewPasswordDto setNewPasswordDto, CancellationToken ct);
    Task<Result<object>> VerifyResetToken(string email, string token, CancellationToken ct);
}
