using Newtonsoft.Json;
using SkillMap.Core.Constants;
using SkillMap.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace SkillMap.Api.Models.Account;

public class RegistrationRequest
{
    [Required]
    [MinLength(6)]
    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;

    public AppUser GetAppUser() => new()
    {
        UserName = Username,
        Email = Email,
        PasswordHash = Password,
        Role = Role.User,
    };
}
