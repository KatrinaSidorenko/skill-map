using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Models;

namespace SkillMap.Api.RoadmapTest;

[Route("api/[controller]")]
[ApiController]
public class RoadmapTestController : BaseController
{
    private IRoadmapTestService _roadmapTestService { get; }
    public RoadmapTestController(IRoadmapTestService roadmapTestService)
    {
        _roadmapTestService = roadmapTestService;
    }
    [HttpPost("{roadmapId}")]
    public async Task<IActionResult> GenerateRoadmapTest(string roadmapId, [FromBody]RoadmapTestConfigDto config, CancellationToken ct)
    {
        var response = await _roadmapTestService.GenerateRoadmapTest(GetUserId(), roadmapId, config, ct);
        return Ok(response);
    }

    [HttpPost("check/{testId}")]
    public async Task<IActionResult> CheckRoadmapTest(string testId, [FromBody] RoadmapTestAnswersRequest userAnswers, CancellationToken ct)
    {
        var response = await _roadmapTestService.CheckRoadmapTest(GetUserId(), testId, userAnswers.ToDto(), ct);
        return Ok(response);
    }
}
