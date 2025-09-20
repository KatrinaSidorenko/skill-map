using Newtonsoft.Json;

namespace SkillMap.Api.Account.Models;

public class UserResponse
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;
}
