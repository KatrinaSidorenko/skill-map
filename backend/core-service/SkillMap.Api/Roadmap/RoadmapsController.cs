using LearningPlatform.Roadmap.Business.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.Base.Searching;
using SkillMap.Api.Roadmap.Models;
using SkillMap.Api.Roadmaps.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Shared.Results;

namespace SkillMap.Api.Roadmaps;

[ApiController]
[Authorize]
public class RoadmapsController : BaseController
{
    private IRoadmapService RoadmapService { get; }
    private IUserRoadmapsService UserRoadmapsService { get; }
    public RoadmapsController(IRoadmapService roadmapService, IUserRoadmapsService userRoadmapsService)
    {
        RoadmapService = roadmapService ?? throw new ArgumentNullException(nameof(roadmapService));
        UserRoadmapsService = userRoadmapsService ?? throw new ArgumentNullException(nameof(userRoadmapsService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlainRoadmaps([FromQuery]SearchingRequest @params, CancellationToken ct)
    {
        var plainRoadmapsResult = await RoadmapService.GetPlainRoadmaps(@params.ToParams(), ct);
        return Response(plainRoadmapsResult, (r) =>
        {
            return Ok(new PaginationResponse<PlainRoadmapResponse>()
            {
                Total = plainRoadmapsResult.Data.TotalCount,
                Items = r.Data.Result.Select(r => r.ToRoadmapResponse()).ToList()
            });
        });
    }

    [HttpGet("{roadmapId}")]
    public async Task<IActionResult> GetRoadmap([FromRoute] string roadmapId, CancellationToken ct)
    {
        var result = await RoadmapService.GetRoadmapById(roadmapId, ct);
        var userRoadmapResult = await UserRoadmapsService.GetUserRoadmaps(GetUserId(), ct);
        var roadmapIds = userRoadmapResult.IsSuccessful ? userRoadmapResult.Data.Select(ur => ur.RoadmapId).ToList() : new List<string>();
        if (result.IsSuccessful && result.Data != null)
        {
            result.Data.IsSaved = roadmapIds.Contains(roadmapId);
        }

        return Response(result, (r) => Ok(new RoadmapResponse
        {
            Roadmap = r.Data
        }));
    }

    [HttpGet("materials/{roadmapId}")]
    public async Task<IActionResult> GetLearningItemMaterials([FromRoute] string roadmapId, [FromQuery] string itemId, CancellationToken ct)
    {
        var mock = new List<LearningItemMaterialResponse> {
            new LearningItemMaterialResponse
            {
                Id = "1",
                Title = "Introduction to C#",
                Type = "Video",
                Url = "https://example.com/intro-to-csharp"
            },
            new LearningItemMaterialResponse
            {
                Id = "2",
                Title = "C# Basics",
                Type = "Article",
                Url = "https://example.com/csharp-basics"
            },
            new LearningItemMaterialResponse
            {
                Id = "3",
                Title = "C# Advanced Topics",
                Type = "Course",
                Url = "https://example.com/csharp-advanced"
            }
         };
        return Ok(mock);
    }
}
