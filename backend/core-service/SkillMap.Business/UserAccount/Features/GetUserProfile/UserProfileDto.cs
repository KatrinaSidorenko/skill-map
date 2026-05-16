namespace SkillMap.Business.UserAccount.Features.GetUserProfile;

public class UserProfileDto
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? ImageUrl { get; set; }
    public string Role { get; set; }
}
