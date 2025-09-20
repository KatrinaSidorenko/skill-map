using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Core.Constants;

namespace SkillMap.Api.UserRoadmaps;

[ApiController]
[Authorize(Roles = Role.User)]
public class UserRoadmapsController : BaseController
{
    private IUserRoadmapsService UserRoadmapsService { get; }
    public UserRoadmapsController(IUserRoadmapsService userRoadmapsService)
    {
        UserRoadmapsService = userRoadmapsService ?? throw new ArgumentNullException(nameof(userRoadmapsService));
    }

    [HttpPost("{roadmapId}")]
    public async Task<IActionResult> AddRoadmap(string roadmapId, CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await UserRoadmapsService.AddRoadmap(userId, roadmapId, ct);
        return Response(result);
    }

    [HttpDelete("{roadmapId}")]
    public async Task<IActionResult> RemoveRoadmap(string roadmapId, CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await UserRoadmapsService.RemoveRoadmap(userId, roadmapId, ct);
        return Response(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRoadmaps(CancellationToken ct)
    {
        var userId = GetUserId();
        var result = await UserRoadmapsService.GetUserRoadmaps(userId, ct);
        return Response(result, (r) =>
        {
            return Ok(new UserRoadmapsResponse
            {
                Roadmaps = r.Data.Select(ur => new UserRoadmapResponse
                {
                    Id = ur.Id,
                    RoadmapId = ur.RoadmapId
                }).ToList()
            });
        });
    }
}
