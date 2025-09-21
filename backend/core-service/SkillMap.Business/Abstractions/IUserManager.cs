using SkillMap.Core.Entities;

namespace SkillMap.Business.Abstractions;

public interface IUserManager
{
    AppUser GetCurrentUser();
}
