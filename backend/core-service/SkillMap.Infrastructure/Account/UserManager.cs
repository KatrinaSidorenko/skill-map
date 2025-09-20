using Microsoft.AspNetCore.Http;
using SkillMap.Core.Entities;
using System.Security.Claims;
using SkillMap.Shared.Extensions;
using SkillMap.Business.Abstractions;

namespace SkillMap.Infrastructure.Account;

public class UserManager : IUserManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserManager(IHttpContextAccessor httpContextAccessor)
    {

        _httpContextAccessor = httpContextAccessor;

    }
    public AppUser GetCurrentUser()
    {
        var claims = _httpContextAccessor?.HttpContext?.User?.Claims;
        if (claims == null || !claims.Any())
        {
            return null;
        }

        var id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value.GetLongOrDefault();
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var role = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();

        if (id == null || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentNullException("User claims are not valid");
        }

        return new AppUser
        {
            Id = id.Value,
            UserName = name,
            Email = email,
            Role = role
        };

    }
}
