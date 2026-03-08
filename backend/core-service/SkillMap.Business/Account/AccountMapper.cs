using Riok.Mapperly.Abstractions;

using SkillMap.Business.Account.Models;
using SkillMap.Core.User;

namespace SkillMap.Business.Account;

[Mapper]
public static partial class AccountMapper
{
    public static partial AppUser ToAppUser(this UserRegistrationDto dto);
    public static partial UserDto ToUserDto(this AppUser user);
}