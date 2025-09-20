namespace SkillMap.Shared.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryInMinutes { get; set; }
}
