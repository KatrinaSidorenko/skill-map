using SkillMap.Business.Abstractions;
using SkillMap.Business.Account.Models;
using SkillMap.Core.Entities;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Account;

public class AccountService(ITokenService tokenService, IRepository<AppUser> userRepository, IPasswordHasher passwordHasher) : IAccountService
{
    public async Task<Result<UserDto>> Login(string email, string password, CancellationToken ct)
    {
        var userResult = await userRepository.GetFirstOrDefaultAsync(x => x.Email == email, ct);
        if (!userResult.IsSuccessful || userResult.Data is null)
        {
            return ResultTypes.UserNotFound<UserDto>(email);
        }

        var isPasswordValid = passwordHasher.Verify(password, userResult.Data.PasswordHash);
        if (!isPasswordValid)
        {
            return ResultTypes.InvalidPassword<UserDto>(email);
        }

        var token = tokenService.GenerateToken(userResult.Data);
        var userDto = new UserDto
        {
            Token = token,
            Email = userResult.Data.Email,
            UserName = userResult.Data.UserName,
        };

        return Result.Success(userDto);
    }

    public async Task<Result<bool>> Register(AppUser appUser, CancellationToken ct)
    {
        var hashedPassword = passwordHasher.Hash(appUser.PasswordHash);
        appUser.PasswordHash = hashedPassword;

        var userResult = await userRepository.GetFirstOrDefaultAsync(x => x.Email == appUser.Email, ct);
        if (userResult?.Data is not null)
        {
            return ResultTypes.UserWithSuchEmailAlreadyExists<bool>(appUser.Email);
        }

        await userRepository.AddAsync(appUser, ct);
        var result = await userRepository.SaveChangesAsync(ct);
        if (!result.IsSuccessful)
        {
            return ResultTypes.FailedToCreateUser<bool>(appUser.Email);
        }

        return Result.Success(true);
    }
}
