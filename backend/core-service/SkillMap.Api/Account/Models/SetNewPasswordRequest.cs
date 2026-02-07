using Newtonsoft.Json;

namespace SkillMap.Api.Account.Models;

public class SetNewPasswordRequest
{
    [JsonProperty]
    public string Token { get; set; }
    [JsonProperty]
    public string Password { get; set; }
}