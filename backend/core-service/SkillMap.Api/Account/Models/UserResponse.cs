using Newtonsoft.Json;

namespace SkillMap.Api.Account.Models;

public class UserResponse
{
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("username")]
    public string UserName { get; set; }
}
