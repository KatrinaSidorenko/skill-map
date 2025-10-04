using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.Base.Searching;
using SkillMap.Api.Roadmaps.Models;
using SkillMap.Business.Roadmaps;

namespace SkillMap.Api.Roadmaps;

[ApiController]
[Authorize]
public class RoadmapsController : BaseController
{
    private IRoadmapService RoadmapService { get; }
    public RoadmapsController(IRoadmapService roadmapService)
    {
        RoadmapService = roadmapService ?? throw new ArgumentNullException(nameof(roadmapService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlainRoadmaps([FromQuery]SearchingRequest @params, CancellationToken ct)
    {
        var plainRoadmapsResult = await RoadmapService.GetPlainRoadmaps(@params.ToParams(), ct);
        return Response(plainRoadmapsResult, (r) =>
        {
            return Ok(new PaginationResponse<PlainRoadmapResponse>()
            {
                TotalCount = plainRoadmapsResult.Data.TotalCount,
                Items = r.Data.Result.Select(r => r.ToRoadmapResponse()).ToList()
            });
        });
    }

    [HttpGet("{roadmapId}")]
    public async Task<IActionResult> GetRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await RoadmapService.GetRoadmapById(roadmapId, ct);
        return Response(result, (r) => Ok(new RoadmapResponse
        {
            Roadmap = r.Data
        }));
    }


    //[HttpGet("saved")]
    //public async Task<IActionResult> GetAll(CancellationToken ct)
    //{
    //    var result = await CustomizedRoadmapsService.GetPlainRoadmapsWithUserMetadata(GetUserId(), ct);

    //    return Response(result, (r) =>
    //    {
    //        return Ok(new PlainRoadmapsResponse
    //        {
    //            Roadmaps = r.Data.Select(ur => ur.ToRoadmapResponse()).ToList()
    //        });
    //    });
    //}

    //[HttpGet("{roadmapId}")]
    //public async Task<IActionResult> Get(string roadmapId, CancellationToken ct)
    //{
    //    var userId = GetUserId();
    //    var result = await CustomizedRoadmapsService.GetRoadmap(userId, roadmapId, ct);

    //    return Response(result, (r) => Ok(new ModifiedRoadmapResponse
    //    {
    //        Roadmap = r.Data.Roadmap,
    //        Nodes = r.Data.LearningItems.Select(i => new ModifiedNodeResponse
    //        {
    //            Id = i.Id,
    //            Title = i.Title,
    //            Type = i.Type,
    //            ParentId = i.ParentId,
    //            Children = i.Children.ToList(),
    //            Progress = i.Progress,
    //            AdditinalProps = i.AdditinalProps,
    //            Description = i.Description,
    //            Status = i.Status,
    //            Index = i.Index,
    //        }).ToList()
    //    }));
    //}

    //[HttpDelete("{roadmapId}")]
    //public async Task<IActionResult> Delete(string roadmapId, [FromQuery]string itemId, CancellationToken ct)
    //{
    //    var userId = GetUserId();
    //    var result = await CustomizedRoadmapsService.DeleteRoadmap(userId, roadmapId, itemId, ct);

    //    return NoContent();
    //}

    //[HttpPost("{roadmapId}/status")]
    //public async Task<IActionResult> UpdateStatus(string roadmapId, [FromQuery] string itemId, [FromQuery] string status, CancellationToken ct)
    //{
    //    var userId = GetUserId();
    //    var metadata = new UpdateStatusMetadata
    //    {
    //        Status = Enum.Parse<LearningStatus>(status),
    //    };
    //    var result = await CustomizedRoadmapsService.UpdateStatus(userId, roadmapId, itemId, metadata, ct);

    //    return NoContent();
    //}

    //[HttpPost("{roadmapId}/edit")]
    //public async Task<IActionResult> Edit(string roadmapId, [FromBody] LearningItemSnapshot learningItem, CancellationToken ct)
    //{
    //    var userId = GetUserId();
    //    var result = await CustomizedRoadmapsService.Update(userId, roadmapId, learningItem, ct);
    //    return NoContent();
    //}

    //[HttpPost("{roadmapId}/create")]
    //public async Task<IActionResult> Create(string roadmapId, [FromBody] CreateLearningItemMetadata learningItem, CancellationToken ct)
    //{
    //    var userId = GetUserId();
    //    var result = await CustomizedRoadmapsService.Create(userId, roadmapId, learningItem, ct);
    //    return Ok(result.Data);
    //}

}
