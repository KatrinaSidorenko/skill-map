using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SkillMap.Business.Abstractions;
using SkillMap.Core.User;
using SkillMap.Shared.Options;

namespace SkillMap.Infrastructure.Account;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    public TokenService(IOptions<JwtOptions> options)
    {
        _jwtOptions = options.Value;
    }
    public string GenerateToken(AppUser identity)
    {
        ArgumentNullException.ThrowIfNull(identity);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, identity.Id.ToString()),
            new (ClaimTypes.Name, identity.UserName!),
            new (ClaimTypes.Email, identity.Email!),
            new (ClaimTypes.Role, identity.Role)
        };

        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryInMinutes);

        var secToken = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expires,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(secToken);

        return token;
    }
}