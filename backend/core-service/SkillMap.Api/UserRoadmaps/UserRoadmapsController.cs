using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.UserRoadmaps.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Core.Constants;

namespace SkillMap.Api.UserRoadmaps;

[ApiController]
//[Authorize(Roles = Role.User)]
public class UserRoadmapsController : BaseController
{
    private IUserRoadmapsService UserRoadmapsService { get; }
    public UserRoadmapsController(IUserRoadmapsService userRoadmapsService)
    {
        UserRoadmapsService = userRoadmapsService ?? throw new ArgumentNullException(nameof(userRoadmapsService));
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveRoadmap([FromQuery]string roadmapId, CancellationToken ct)
    {
        var result = await UserRoadmapsService.LinkRoadmap(GetUserId(), roadmapId, ct);
        return Response(result);
    }

    [HttpDelete("{roadmapId}")]
    public async Task<IActionResult> RemoveRoadmap(string roadmapId, CancellationToken ct)
    {
        var result = await UserRoadmapsService.RemoveRoadmap(GetUserId(), roadmapId, ct);
        return Response(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRoadmaps(CancellationToken ct)
    {
        var result = await UserRoadmapsService.GetUserRoadmaps(GetUserId(), ct);
        return Response(result, (r) =>
        {
            return Ok(new UserRoadmapsResponse
            {
                Roadmaps = r.Data.Select(ur => ur.ToUserRoadmapResponse()).ToList()
            });
        });
    }
}
