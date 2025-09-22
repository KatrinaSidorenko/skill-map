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
    IValidator<LoginDto> loginValidator,
    IValidator<UserRegistrationDto> userValidator,
    IResetAccountService resetAccountService,
    IPasswordHasher passwordHasher) : IAccountService
{
    public async Task<Result<UserDto>> Login(LoginDto loginDto, CancellationToken ct)
    {
        var validationResult = await loginValidator.ValidateAsync(loginDto, ct);
        if (!validationResult.IsValid)
        {
            return ResultType.ValidationError<UserDto>(validationResult.GetErrors());
        }

        var email = loginDto.Email;
        var password = loginDto.Password;
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
        var userDto = userResult.Data.ToUserDto();
        userDto.Token = token;

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
        var appUser = userDto.ToAppUser();
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

    public async Task<Result<bool>> ResetPassword(string email, CancellationToken ct)
    {
        var userResult = await userRepository.GetFirstOrDefaultAsync(x => x.Email == email, ct);
        if (!userResult.IsSuccessful || userResult.Data is null)
        {
            return ResultType.UserNotFound<bool>(email);
        }

        return await resetAccountService.ResetPassword(email, ct);
    }

    public async Task<Result<bool>> SetNewPassword(SetNewPasswordDto setNewPasswordDto, CancellationToken ct)
    {
        var emailResult = await resetAccountService.GetEmailByToken(setNewPasswordDto.Token, ct);
        if (!emailResult.IsSuccessful || emailResult.Data is null)
        {
            return ResultType.InvalidOrExpiredToken<bool>();
        }
        var email = emailResult.Data;
        var userResult = await userRepository.GetFirstOrDefaultAsync(x => x.Email == email, ct);
        if (!userResult.IsSuccessful || userResult.Data is null)
        {
            return ResultType.UserNotFound<bool>(email);
        }
        var user = userResult.Data;
        user.PasswordHash = passwordHasher.Hash(setNewPasswordDto.Password);
        await userRepository.UpdateAsync(user, ct);
        var result = await userRepository.SaveChangesAsync(ct);
        if (!result.IsSuccessful)
        {
            return ResultType.FailedToUpdatePassword<bool>(email);
        }
        return Result.Success(true);
    }

    public async Task<Result<bool>> VerifyResetToken(string token, CancellationToken ct)
         => await resetAccountService.VerifyResetToken(token, ct);
}
