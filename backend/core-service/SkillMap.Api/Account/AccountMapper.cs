using Riok.Mapperly.Abstractions;

using SkillMap.Api.Account.Models;
using SkillMap.Business.Account.Models;
using SkillMap.Core.Entities;

namespace SkillMap.Api.Account;

[Mapper]
public static partial class AccountMapper
{
    [MapProperty(nameof(RegistrationRequest.Username), nameof(UserRegistrationDto.UserName))]
    public static partial UserRegistrationDto ToUserRegistrationDto(this RegistrationRequest registrationRequest);
    public static partial LoginDto ToLoginDto(this LoginRequest loginRequest);

    [MapProperty(nameof(UserDto), nameof(LoginResponse.User))]
    public static partial LoginResponse ToLoginResponse(this UserDto userDto);

    [MapProperty(nameof(UserDto.UserName), nameof(UserResponse.Username))]
    public static partial UserResponse ToUserResponse(this UserDto userDto);

    [MapProperty(nameof(AppUser.UserName), nameof(UserResponse.Username))]
    public static partial UserResponse ToUserResponse(this AppUser userDto);
    public static partial SetNewPasswordDto ToSetNewPasswordDto(this SetNewPasswordRequest setNewPasswordRequest);
}