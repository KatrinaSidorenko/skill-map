using Newtonsoft.Json;

namespace SkillMap.Api.Account.Models;

public class ResetPasswordRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }
}
