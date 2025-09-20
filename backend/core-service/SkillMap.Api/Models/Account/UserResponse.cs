using Newtonsoft.Json;

namespace SkillMap.Api.Models.Account;

public class UserResponse
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;
}
