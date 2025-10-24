using LearningPlatform.Roadmap.Business;
using LearningPlatform.Roadmap.Business.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.Base.Searching;
using SkillMap.Api.Roadmap.Models;
using SkillMap.Api.Roadmaps;
using SkillMap.Api.Roadmaps.Models;
using SkillMap.Api.UserRoadmaps.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Core.Constants;

namespace SkillMap.Api.UserRoadmaps;

[ApiController]
//[Authorize(Roles = Role.User)]
public class UserRoadmapsController : BaseController
{
    private IUserRoadmapsService UserRoadmapsService { get; }
    private IRoadmapService RoadmapService { get; }
    public UserRoadmapsController(IUserRoadmapsService userRoadmapsService, IRoadmapService roadmapService)
    {
        UserRoadmapsService = userRoadmapsService ?? throw new ArgumentNullException(nameof(userRoadmapsService));
        RoadmapService = roadmapService ?? throw new ArgumentNullException(nameof(roadmapService));
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

    [HttpPost]
    public async Task<IActionResult> CreateRoadmap([FromBody] CreatePlainRoadmapRequest request, CancellationToken ct)
    {
        var dto = request.ToPlainRoadmapDto();
        dto.OwnerId = GetUserId().ToString();
        dto.IsPublic = request.IsPublic;

        var result = await UserRoadmapsService.CreateUserRoadmap(GetUserId(), dto, ct);
        return Response(result, (r) => Ok(new { id = result.Data }));
    }

    [HttpPost("node")]
    public async Task<IActionResult> CreateNode([FromBody] CreateNodeRequest request, CancellationToken ct)
    {
        await RoadmapService.CreateNode(request.NodeDto, ct);
        return NoContent();
    }

    [HttpPost("edge")]
    public async Task<IActionResult> CreateEdge([FromBody] CreateEdgeRequest request, CancellationToken ct)
    {
        await RoadmapService.CreateEdge(request.EdgeDto, ct);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRoadmaps([FromQuery]SearchingRequest @params, CancellationToken ct)
    {
        var plainRoadmapsResult = await UserRoadmapsService.GetUserCreatedRoadmaps(GetUserId(), @params.ToParams(), ct);
        return Response(plainRoadmapsResult, (r) =>
        {
            return Ok(new PaginationResponse<PlainRoadmapResponse>()
            {
                Total = plainRoadmapsResult.Data.TotalCount,
                Items = r.Data.Result.Select(r => r.ToRoadmapResponse()).ToList()
            });
        });
    }
}
