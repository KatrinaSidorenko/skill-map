using SkillMap.Business.Account.Models;
using SkillMap.Core.Entities;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Account;

public interface IAccountService
{
    Task<Result<UserDto>> Login(LoginCommand loginDto, CancellationToken ct);
    Task<Result<bool>> Register(UserRegistrationDto userDto, CancellationToken ct);
    Task<Result<bool>> ResetPassword(string email, CancellationToken ct);
    Task<Result<bool>> SetNewPassword(SetNewPasswordDto setNewPasswordDto, CancellationToken ct);
    Task<Result<bool>> VerifyResetToken(string token, CancellationToken ct);
}