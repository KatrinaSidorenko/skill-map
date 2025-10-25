using LearningPlatform.Roadmap.Business;
using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.Base.Searching;
using SkillMap.Api.ModifiedRoadmap.Models;
using SkillMap.Api.Roadmap.Models;
using SkillMap.Api.Roadmaps;
using SkillMap.Api.Roadmaps.Models;
using SkillMap.Api.UserRoadmaps.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Core.Constants;
using CreateEdgeRequest = SkillMap.Api.ModifiedRoadmap.Models.CreateEdgeRequest;
using CreateNodeRequest = SkillMap.Api.Roadmap.Models.CreateNodeRequest;

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

    [HttpPost("create-item/{roadmapId}")]
    public async Task<IActionResult> CreateLearningItem([FromRoute]string roadmapId, [FromBody] CreateNodeRequest request, CancellationToken ct)
    {
        var nodeDto = request.ToNodeDto(roadmapId);
        await RoadmapService.CreateNode(roadmapId, nodeDto, ct);
        return NoContent();
    }

    [HttpPost("create-connection/{roadmapId}")]
    public async Task<IActionResult> CreateLearningItemConnection([FromRoute]string roadmapId, [FromBody] CreateEdgeRequest request, CancellationToken ct)
    {
        var edgeDto = new EdgeDto
        {
            Source = new NodeDto { Id = request.SourceId },
            Target = new NodeDto { Id = request.TargetId }
        };
        await RoadmapService.CreateEdge(edgeDto, ct);
        return NoContent();
    }

    [HttpPost("delete-item/{roadmapId}")]
    public async Task<IActionResult> DeleteLearningItem([FromRoute] string roadmapId, [FromBody] DeleteLearningItemRequest request, CancellationToken ct)
    {
        await RoadmapService.DeleteRoadmapElement(roadmapId, request.Id, ct);
        return Ok();
    }

    [HttpPost("update-item/{roadmapId}")]
    public async Task<IActionResult> UpdateLearningItem([FromRoute] string roadmapId, [FromBody] LearningItemChangeRequest request, CancellationToken ct)
    {
        // we need to update the learning item node
        // for now we will assume it exists and just update it
        return Ok();
    }

    [HttpGet("{roadmapId}")]
    public async Task<IActionResult> GetUserRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await RoadmapService.GetRoadmapById(roadmapId, ct, includeStartNode: false);
        return Response(result, (r) => Ok(new RoadmapResponse
        {
            Roadmap = r.Data
        }));
    }
}
