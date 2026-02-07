using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace SkillMap.Api.Account.Models;

public class LoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("password")]
    public string Password { get; set; }
}