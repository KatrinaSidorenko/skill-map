using SkillMap.Core.User;

namespace SkillMap.Business.Abstractions;

public interface IUserManager
{
    AppUser GetCurrentUser();
}