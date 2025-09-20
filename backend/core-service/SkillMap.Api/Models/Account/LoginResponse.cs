using Newtonsoft.Json;

namespace SkillMap.Api.Models.Account;

public class LoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("user")]
    public UserResponse User { get; set; }
}
