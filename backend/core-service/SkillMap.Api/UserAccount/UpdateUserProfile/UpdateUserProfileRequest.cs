using System.Text.Json.Serialization;

namespace SkillMap.Api.UserAccount.UpdateUserProfile;

public class UpdateUserProfileRequest
{
    [JsonPropertyName("username")]
    public string? UserName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
