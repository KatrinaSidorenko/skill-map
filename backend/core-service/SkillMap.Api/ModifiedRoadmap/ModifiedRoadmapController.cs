using LearningPlatform.Shared.Api.Searching;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SkillMap.Api.Base;
using SkillMap.Api.ModifiedRoadmap.Models;
using SkillMap.Business.Roadmaps;

namespace SkillMap.Api.ModifiedRoadmap;

[ApiController]
[Authorize]
public class ModifiedRoadmapsController(ICustomizedRoadmapsService customizedRoadmapsService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SearchingRequest request, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.GetPlainRoadmapsWithUserMetadata(GetUserId(), request.ToParams(), ct);
        return HandleResponse(result, (r) =>
        {
            return Ok(new PaginationResponse<SavedPlainRoadmapResponse>()
            {
                Total = result.Data.TotalCount,
                Items = r.Data.Result.Select(r => r.ToResponse()).ToList()
            });
        });
    }

    [HttpGet("{roadmapId}")]
    public async Task<IActionResult> GetSavedRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.GetUserModifiedRoadmap(GetUserId(), roadmapId, ct);
        return HandleResponse(result, (r) => Ok(result.Data));
    }

    [HttpGet("plain/{roadmapId}")]
    public async Task<IActionResult> GetSavedPlainRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.GetPlainRoadmapWithUserMetadata(GetUserId(), roadmapId, ct);
        return HandleResponse(result, (r) => Ok(r.Data.ToResponse()));
    }

    [HttpPost("save-change/{roadmapId}")]
    public async Task<IActionResult> SaveChanges([FromRoute] string roadmapId, [FromBody] LearningItemsChangesRequest changesRequest, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.SaveLearningItemsChanges(GetUserId(), roadmapId, changesRequest.Changes.Select(c => c.ToChange()).ToList(), ct);
        return HandleResponse(result, (r) => NoContent());
    }

    [HttpPost("delete/{roadmapId}")]
    public async Task<IActionResult> DeleteSavedRoadmap([FromRoute] string roadmapId, [FromBody] DeleteLearningItemRequest deleteLearningItem, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.SaveDeleteItemChange(GetUserId(), roadmapId, deleteLearningItem.ToChange(), ct);
        return HandleResponse(result, (r) => NoContent());
    }

    [HttpPost("create-item/{roadmapId}")]
    public async Task<IActionResult> CreateNode([FromRoute] string roadmapId, [FromBody] CreateNodeRequest itemMetadata, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.CreateLearningItem(GetUserId(), roadmapId, itemMetadata.ToLearningItem(), ct);
        return HandleResponse(result, (r) => NoContent());
    }

    [HttpPost("create-connection/{roadmapId}")]
    public async Task<IActionResult> CreateConnection([FromRoute] string roadmapId, [FromBody] CreateEdgeRequest connectionMetadata, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.CreateLearningItemsConnection(GetUserId(), roadmapId, connectionMetadata.ToLearningItemConnection(), ct);
        return HandleResponse(result, (r) => NoContent());
    }
}