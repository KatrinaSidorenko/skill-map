using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Models.Account;
using SkillMap.Business.Account;

namespace SkillMap.Api.Controllers;

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
        var result = await AccountService.Register(registrationRequest.GetAppUser(), ct);
        return Response(result, r => Ok());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest, CancellationToken ct)
    {
        var result = await AccountService.Login(loginRequest.Email, loginRequest.Password, ct);
        return Response(result, r => Ok(new LoginResponse
        {
            Token = r.Data.Token,
            User = new UserResponse
            {
                Email = r.Data.Email,
                UserName = r.Data.UserName,
            }
        }));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var user = CurrentUser;
        return Ok(new UserResponse
        {
            Email = user.Email,
            UserName = user.UserName,
        });
    }
}
