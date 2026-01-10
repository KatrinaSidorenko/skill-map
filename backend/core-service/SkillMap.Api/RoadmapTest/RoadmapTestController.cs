using LearningPlatform.Roadmap.Business;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Api.Base;
using SkillMap.Api.Roadmaps.Models;
using SkillMap.Api.RoadmapTest.Models;
using SkillMap.Application.Services;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Business.UserTest;
using SkillMap.Shared.Results;

namespace SkillMap.Api.RoadmapTest;

[Route("api/[controller]")]
[ApiController]
public class RoadmapTestController : BaseController
{
    private readonly IRoadmapTestService _roadmapTestService;
    private readonly IUserRoadmapTestService _userRoadmapTestService;

    public RoadmapTestController(IRoadmapTestService roadmapTestService, IUserRoadmapTestService userRoadmapTestService)
    {
        _roadmapTestService = roadmapTestService;
        _userRoadmapTestService = userRoadmapTestService;
    }

    [HttpPost("{roadmapId}/initial")]
    public async Task<IActionResult> CreateInitialRoadmapTest([FromRoute]string roadmapId, [FromBody]RoadmapTestConfigDto config, CancellationToken ct)
    {
        var response = await _roadmapTestService.CreateInitialRoadmapTest(GetUserId(), roadmapId, config, ct);
        return Ok(response);
    }

    [HttpGet("{testId}/start")]
    public async Task<IActionResult> StartTakingRoadmapTest(string testId, CancellationToken ct)
    {
        var testResultId = await _userRoadmapTestService.SaveStartOfTakingRoadmapTest(testId, ct);
        return Ok(testResultId);
    }


    [HttpPost("check/{testId}")]
    public async Task<IActionResult> CheckRoadmapTest(string testId, [FromBody] RoadmapTestAnswersRequest userAnswers, CancellationToken ct)
    {
        var testResultId = await _roadmapTestService.EstimateRoadmapTest(testId, userAnswers.ToDto(), ct);
        return Ok(testResultId);
    }

    [HttpGet("{testId}")]
    public async Task<IActionResult> GetUserRoadmapTest(string testId, CancellationToken ct)
    {
        var roadmapTest = await _userRoadmapTestService.GetRoadmapTest(testId, ct);
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
    public async Task<IActionResult> GetRoadmapChangesSuggestions(string testResultId, CancellationToken ct)
        => Ok(await _roadmapTestService.CreateRoadmapChangesSuggestions(GetUserId(), testResultId, ct));

    [HttpGet("applysuggestions/{testId}")]
    public async Task<IActionResult> ApplyRoadmapChangesSuggestions(string testId, CancellationToken ct)
    {
        //var response = await _roadmapTestService.ApplyRoadmapChangesSuggestions(GetUserId(), testId, ct);
        //return Ok(response);
        return Ok();
    }
}
