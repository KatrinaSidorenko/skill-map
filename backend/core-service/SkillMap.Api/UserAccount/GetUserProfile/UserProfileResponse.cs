using System.Text.Json.Serialization;

namespace SkillMap.Api.UserAccount.GetUserProfile;

public class UserProfileResponse
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("username")]
    public string UserName { get; init; }

    [JsonPropertyName("email")]
    public string Email { get; init; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; init; }

    [JsonPropertyName("role")]
    public string Role { get; init; }
}
