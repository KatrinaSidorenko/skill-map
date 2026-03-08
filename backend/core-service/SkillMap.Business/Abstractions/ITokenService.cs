using SkillMap.Core.User;

namespace SkillMap.Business.Abstractions;

public interface ITokenService
{
    string GenerateToken(AppUser identity);
}