using Newtonsoft.Json;
using SkillMap.Core.Constants;
using SkillMap.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace SkillMap.Api.Account.Models;

public class RegistrationRequest
{
    [Required]
    [JsonProperty("username")]
    public string Username { get; set; } 
    [Required]
    [JsonProperty("email")]
    public string Email { get; set; } 
    [Required]
    [JsonProperty("password")]
    public string Password { get; set; }
    [JsonProperty("role")]
    public string Role { get; set; }
}
