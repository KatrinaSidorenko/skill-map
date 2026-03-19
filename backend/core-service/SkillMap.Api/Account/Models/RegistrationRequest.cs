using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using SkillMap.Core.Constants;

namespace SkillMap.Api.Account.Models;

public class RegistrationRequest
{
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("password")]
    public string Password { get; set; }
    [JsonProperty("role")]
    public string Role { get; set; }
}