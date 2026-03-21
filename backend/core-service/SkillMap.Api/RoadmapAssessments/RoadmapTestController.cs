using Microsoft.AspNetCore.Mvc;

using SkillMap.Api.Base;
using SkillMap.Api.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmapTest.Models;
using SkillMap.Business.UserTest;

namespace SkillMap.Api.RoadmapTest;

[Route("api/[controller]")]
[ApiController]
public class RoadmapTestController : BaseController
{
    private readonly IRoadmapAssessmentService _roadmapTestService;
    private readonly IAssessmentAttemptService _userRoadmapTestService;

    public RoadmapTestController(IRoadmapAssessmentService roadmapTestService, IAssessmentAttemptService userRoadmapTestService)
    {
        _roadmapTestService = roadmapTestService;
        _userRoadmapTestService = userRoadmapTestService;
    }

    [HttpPost("{roadmapId}/initial")]
    public async Task<IActionResult> CreateInitialRoadmapTest([FromRoute] string roadmapId, [FromBody] RoadmapTestConfigDto config, CancellationToken ct)
        => HandleResponse(await _roadmapTestService.CreateInitialRoadmapTest(GetUserId(), roadmapId, config, ct), (r) => Ok(r));

    [HttpPost("{roadmapId}/intermediate")]
    public async Task<IActionResult> CreateIntermediateRoadmapTest([FromRoute] string roadmapId, [FromBody] RoadmapTestConfigDto config, CancellationToken ct)
        => HandleResponse(await _roadmapTestService.CreateIntermediateRoadmapTest(GetUserId(), roadmapId, config, ct), (r) => Ok(r));

    [HttpGet("history/{workspaceId}")]
    public async Task<TestingHistoryDto> GetRoadmapTestingHistory(long workspaceId, CancellationToken ct)
        => await _userRoadmapTestService.GetRoadmapTestingHistory(workspaceId, ct);

    [HttpPost("{testId}/start")]
    public async Task<IActionResult> StartTakingRoadmapTest(string testId, CancellationToken ct)
    {
        var testResultId = await _userRoadmapTestService.SaveStartOfTakingRoadmapTest(testId, ct);
        return Ok(new TestResultResponse(testResultId));
    }


    [HttpPost("check/{testId}")]
    public async Task<IActionResult> CheckRoadmapTest(string testId, [FromBody] RoadmapTestAnswersRequest userAnswers, CancellationToken ct)
    {
        var testResultId = await _roadmapTestService.EstimateRoadmapTest(testId, userAnswers.ToDto(), ct);
        return Ok(new TestResultResponse(testResultId));
    }

    [HttpGet("{testId}")]
    public async Task<IActionResult> GetUserRoadmapTest(string testId, CancellationToken ct)
    {
        var roadmapTest = await _userRoadmapTestService.GetRoadmapTest(testId, ct);
        var testResultId = await _userRoadmapTestService.SaveStartOfTakingRoadmapTest(testId, ct);
        return Ok(roadmapTest.ToTestResult(testId));
    }

    [HttpGet("results/{testResultId}")]
    public async Task<IActionResult> GetComplexTestCheck(string testResultId, CancellationToken ct)
    {
        var testAnalysisResult = await _userRoadmapTestService.GetRoadmapTestAnalysisResult(testResultId, ct);
        return Ok(testAnalysisResult);
    }

    //[HttpGet("rebuild/{roadmapId}")]
    //public async Task<IActionResult> RebuildUserTest(string roadmapId, CancellationToken ct)
    //{
    //    var response = await _roadmapTestService.RebuildRoadmapBasedOnTestResults(GetUserId(), roadmapId, ct);
    //    return Ok(response);
    //}

    [HttpGet("suggestions/{testResultId}")]
    public async Task<RoadmapChangesSuggestionsDto> GetRoadmapChangesSuggestions(string testResultId, CancellationToken ct)
        => await _roadmapTestService.GetRoadmapChangesSuggestions(GetUserId(), testResultId, ct);

    [HttpGet("applysuggestions/{testId}")]
    public async Task<IActionResult> ApplyRoadmapChangesSuggestions(string testId, CancellationToken ct)
    {
        //var response = await _roadmapTestService.ApplyRoadmapChangesSuggestions(GetUserId(), testId, ct);
        //return Ok(response);
        return Ok();
    }
}