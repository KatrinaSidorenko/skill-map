using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Business.Abstractions;
using SkillMap.Core.Entities;
using SkillMap.Shared.Results;

namespace SkillMap.Api.Base;

[Route("api/[controller]")]

public class BaseController : ControllerBase
{
    private IUserManager _identityManager;
    protected IUserManager IdentityManager =>
        _identityManager ??= HttpContext.RequestServices.GetService<IUserManager>();

    protected AppUser CurrentUser => IdentityManager.GetCurrentUser();
    protected long GetUserId()
    {
        if (CurrentUser == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return CurrentUser.Id;
    }
    protected IActionResult InternalServerError<T>(Result<T> result) => StatusCode(500, result.GetResultResponse());
    protected IActionResult BadRequest<T>(Result<T> result) => StatusCode(400, result.GetResultResponse());
    protected IActionResult HandleResponse<T>(Result<T> result, Func<Result<T>, IActionResult>? onSuccess = null)
    {
        if (result.IsBadRequest())
        {
            return BadRequest(result);
        }
        
        if (result.IsInternalError())
        {
            return InternalServerError(result);
        }

        if (result.Data is null)
        {
            return NoContent();
        }

        if (onSuccess == null)
        {
            return Ok();
        }

        return onSuccess(result);
    }
}
