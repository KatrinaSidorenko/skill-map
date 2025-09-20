using FluentValidation;
using SkillMap.Business.Abstractions;
using SkillMap.Business.Account.Models;
using SkillMap.Core.Entities;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.Account;

public class AccountService(
    ITokenService tokenService, 
    IRepository<AppUser> userRepository, 
    IValidator<UserRegistrationDto> userValidator,
    IPasswordHasher passwordHasher) : IAccountService
{
    public async Task<Result<UserDto>> Login(string email, string password, CancellationToken ct)
    {
        var userResult = await userRepository.GetFirstOrDefaultAsync(x => x.Email == email, ct);
        if (!userResult.IsSuccessful || userResult.Data is null)
        {
            return ResultType.UserNotFound<UserDto>(email);
        }

        var isPasswordValid = passwordHasher.Verify(password, userResult.Data.PasswordHash);
        if (!isPasswordValid)
        {
            return ResultType.InvalidPassword<UserDto>(email);
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

    public async Task<Result<bool>> Register(UserRegistrationDto userDto, CancellationToken ct)
    {
        var validationResult = await userValidator.ValidateAsync(userDto, ct);
        if (!validationResult.IsValid)
        {
            return ResultType.ValidationError<bool>(validationResult.GetErrors());
        }

        var hashedPassword = passwordHasher.Hash(userDto.Password);
        var appUser = AccountMapper.ToAppUser(userDto);
        appUser.PasswordHash = hashedPassword;

        var userResult = await userRepository.GetFirstOrDefaultAsync(x => x.Email == appUser.Email, ct);
        if (userResult?.Data is not null)
        {
            return ResultType.UserWithSuchEmailAlreadyExists<bool>(appUser.Email);
        }

        await userRepository.AddAsync(appUser, ct);
        var result = await userRepository.SaveChangesAsync(ct);
        if (!result.IsSuccessful)
        {
            return ResultType.FailedToCreateUser<bool>(appUser.Email);
        }

        return Result.Success(true);
    }
}
