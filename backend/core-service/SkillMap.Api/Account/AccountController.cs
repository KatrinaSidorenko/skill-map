using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Account.Models;
using SkillMap.Api.Base;
using SkillMap.Business.Account;

namespace SkillMap.Api.Account;

[ApiController]
public class AccountController : BaseController
{
    private IAccountService AccountService { get; }
    public AccountController(IAccountService accountService)
    {
        AccountService = accountService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegistrationRequest registrationRequest, CancellationToken ct)
    {
        var result = await AccountService.Register(registrationRequest.ToUserRegistrationDto(), ct);
        return HandleResponse(result, r => Ok());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest, CancellationToken ct)
    {
        var result = await AccountService.Login(loginRequest.ToLoginDto(), ct);
        return HandleResponse(result, r => Ok(r.Data.ToLoginResponse()));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest passwordRequest, CancellationToken ct)
    {
        var result = await AccountService.ResetPassword(passwordRequest.Email, ct);
        return HandleResponse(result, r => Ok());
    }

    [HttpGet("verify-token")]
    public async Task<IActionResult> VerifyResetToken([FromQuery]string token, CancellationToken ct)
    {
        var result = await AccountService.VerifyResetToken(token, ct);
        return HandleResponse(result, r => Ok());
    }

    [HttpPost("set-new-password")]
    public async Task<IActionResult> SetNewPassword([FromBody]SetNewPasswordRequest setNewPasswordRequest, CancellationToken ct)
    {
        var result = await AccountService.SetNewPassword(setNewPasswordRequest.ToSetNewPasswordDto(), ct);
        return HandleResponse(result, r => Ok());
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct) => Ok(CurrentUser.ToUserResponse());
}
