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
        return Response(result, r => Ok());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest, CancellationToken ct)
    {
        var result = await AccountService.Login(loginRequest.ToLoginDto(), ct);
        return Response(result, r => Ok(r.Data.ToLoginResponse()));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct) => Ok(CurrentUser.ToUserResponse());
}
