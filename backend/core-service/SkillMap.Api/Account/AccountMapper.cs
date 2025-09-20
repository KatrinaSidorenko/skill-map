using Riok.Mapperly.Abstractions;
using SkillMap.Api.Account.Models;
using SkillMap.Business.Account.Models;

namespace SkillMap.Api.Account;

[Mapper]
public static partial class AccountMapper
{
    [MapProperty(nameof(RegistrationRequest.Username), nameof(UserRegistrationDto.UserName))]
    public static partial UserRegistrationDto ToUserRegistrationDto(this RegistrationRequest registrationRequest);
}
