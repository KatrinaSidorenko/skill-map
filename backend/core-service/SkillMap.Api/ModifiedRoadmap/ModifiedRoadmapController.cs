using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.Base.Searching;
using SkillMap.Api.ModifiedRoadmap.Models;
using SkillMap.Business.Roadmaps;

namespace SkillMap.Api.ModifiedRoadmap;

[ApiController]
[Authorize]
public class ModifiedRoadmapsController(ICustomizedRoadmapsService customizedRoadmapsService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery]SearchingRequest request, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.GetPlainRoadmapsWithUserMetadata(GetUserId(), request.ToParams(), ct);
        return Response(result, (r) =>
        {
            return Ok(new PaginationResponse<SavedPlainRoadmapResponse>()
            {
                TotalCount = result.Data.TotalCount,
                Items = r.Data.Result.Select(r => r.ToResponse()).ToList()
            });
        });
    }

    [HttpGet("{roadmapId}")]
    public async Task<IActionResult> GetSavedRoadmap([FromRoute]string roadmapId, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.GetRoadmap(GetUserId(), roadmapId, ct);
        return Response(result, (r) => Ok(result.Data));
    }

    [HttpPost("save-change/{roadmapId}")]
    public async Task<IActionResult> SaveChanges([FromRoute] string roadmapId, [FromBody]LearningItemChangeRequest item, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.SaveLearningItemChange(GetUserId(), roadmapId, item.ToChange(), ct);
        return Response(result, (r) => NoContent());
    }

    [HttpPost("delete/{roadmapId}")]
    public async Task<IActionResult> DeleteSavedRoadmap([FromRoute] string roadmapId, [FromBody]DeleteLearningItemRequest deleteLearningItem, CancellationToken ct)
    {
        var result = await customizedRoadmapsService.SaveDeleteItemChange(GetUserId(), roadmapId, deleteLearningItem.ToChange(),  ct);
        return Response(result, (r) => NoContent());
    }
}
