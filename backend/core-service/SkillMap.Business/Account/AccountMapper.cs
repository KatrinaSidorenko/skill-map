using Riok.Mapperly.Abstractions;
using SkillMap.Business.Account.Models;
using SkillMap.Core.Entities;

namespace SkillMap.Business.Account;

[Mapper]
public static partial class AccountMapper
{
    public static partial AppUser ToAppUser(this UserRegistrationDto dto);
}
