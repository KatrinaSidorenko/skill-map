using LearningPlatform.Roadmap.Business;
using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.Shared.Api.Searching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.ModifiedRoadmap.Models;
using SkillMap.Api.Roadmap.Models;
using SkillMap.Api.Roadmaps;
using SkillMap.Api.Roadmaps.Models;
using SkillMap.Api.UserRoadmaps.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Core.Constants;
using CreateEdgeRequest = SkillMap.Api.ModifiedRoadmap.Models.CreateEdgeRequest;
//using CreateNodeRequest = SkillMap.Api.Roadmap.Models.CreateNodeRequest;

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
        return HandleResponse(result);
    }

    [HttpDelete("archive/{roadmapId}")]
    public async Task<IActionResult> RemoveRoadmap(string roadmapId, CancellationToken ct)
    {
        var result = await UserRoadmapsService.RemoveRoadmap(GetUserId(), roadmapId, ct);
        return HandleResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoadmap([FromBody] CreatePlainRoadmapRequest request, CancellationToken ct)
    {
        var dto = request.ToPlainRoadmapDto();
        dto.OwnerId = GetUserId().ToString();
        dto.IsPublic = request.IsPublic;

        var result = await UserRoadmapsService.CreateUserRoadmap(GetUserId(), dto, ct);
        return HandleResponse(result, (r) => Ok(new { id = result.Data }));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRoadmaps([FromQuery]SearchingRequest @params, CancellationToken ct)
    {
        var plainRoadmapsResult = await UserRoadmapsService.GetUserCreatedRoadmaps(GetUserId(), @params.ToParams(), ct);
        return HandleResponse(plainRoadmapsResult, (r) =>
        {
            return Ok(new PaginationResponse<PlainRoadmapResponse>()
            {
                Total = plainRoadmapsResult.Data.TotalCount,
                Items = r.Data.Result.Select(r => r.ToRoadmapResponse()).ToList()
            });
        });
    }

    //[HttpPost("create-item/{roadmapId}")]
    //public async Task<IActionResult> CreateLearningItem([FromRoute]string roadmapId, [FromBody]CreateNodeRequest request, CancellationToken ct)
    //{
    //    var nodeDto = request.ToNodeDto(roadmapId);
    //    await RoadmapService.CreateNode(roadmapId, nodeDto, ct);
    //    return NoContent();
    //}

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
        await RoadmapService.DeleteRoadmapElement(roadmapId, request.Id, request.Type, ct);
        return Ok();
    }

    [HttpPost("update-item/{roadmapId}")]
    public async Task<IActionResult> UpdateLearningItem([FromRoute] string roadmapId, [FromBody] LearningItemChangeRequest request, CancellationToken ct)
    {
        var nodeDto = request.ToNodeDto();
        await RoadmapService.UpdateNode(nodeDto, ct);
        return Ok();
    }

    [HttpGet("{roadmapId}")]
    public async Task<IActionResult> GetUserRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await RoadmapService.GetRoadmapById(roadmapId, ct, includeStartNode: false);
        return HandleResponse(result, (r) => Ok(new RoadmapResponse
        {
            Roadmap = r.Data
        }));
    }

    [HttpPut("{roadmapId}")]
    public async Task<IActionResult> UpdateRoadmap([FromRoute] string roadmapId, [FromBody] UpdatePlainRoadmapRequest request, CancellationToken ct)
    {
        var dto = request.ToRoadmapNodeDto(roadmapId, GetUserId().ToString());
        await RoadmapService.UpdateNode(dto, ct);
        return NoContent();
    }

    [HttpDelete("{roadmapId}")]
    public async Task<IActionResult> DeleteRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await UserRoadmapsService.RemoveRoadmap(GetUserId(), roadmapId, ct);
        if (!result.IsSuccessful)
        {
            return HandleResponse(result);
        }
        
        await RoadmapService.DeleteRoadmap(roadmapId, ct);
        return NoContent();
    }

    [HttpGet("plain/{roadmapId}")]
    public async Task<IActionResult> GetPlainRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await UserRoadmapsService.GetCreatedUserRoadmap(GetUserId(), roadmapId, ct);
        return HandleResponse(result, (r) => Ok(r.Data));
    }
}
