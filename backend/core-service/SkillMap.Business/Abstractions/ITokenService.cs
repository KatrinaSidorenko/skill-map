using SkillMap.Core.Entities;

namespace SkillMap.Business.Abstractions;

public interface ITokenService
{
    string GenerateToken(AppUser identity);
}
