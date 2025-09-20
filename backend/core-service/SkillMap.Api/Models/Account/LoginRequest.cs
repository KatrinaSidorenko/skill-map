using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SkillMap.Api.Models.Account;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
}
