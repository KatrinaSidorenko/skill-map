using SkillMap.Business.Account.Models;
using SkillMap.Core.Entities;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Account;

public interface IAccountService
{
    Task<Result<bool>> Register(AppUser appUser, CancellationToken ct);
    Task<Result<UserDto>> Login(string email, string password, CancellationToken ct);
}
