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

    [HttpGet("complex-check/{testId}")]
    public async Task<IActionResult> ComplexCheckRoadmapTest(string testId, CancellationToken ct)
    {
        var test = new ComplexTestCheckResult
        {
            TotalAchievedPoints = 7.0,
            TotalPossiblePoints = 10.0,

            QuestionResults = new Dictionary<string, TestQuestionResult>
            {
                {
                    "q1",
                    new TestQuestionResult
                    {
                        QuestionId = "q1",
                        Text = "What is the capital of France?",
                        IsCorrect = true,
                        AchievedPoints = 5,
                        TotalPossiblePoints = 5,
                        Type = "single_choice",

                        AnswerDetails = new Dictionary<string, AnswerDetail>
                        {
                            {
                                "a1",
                                new SingleChoiceAnswerDetail
                                {
                                    AnswerId = "a1",
                                    Text = "Paris",
                                    IsCorrect = true,
                                    IsSelected = true
                                }
                            },
                            {
                                "a2",
                                new SingleChoiceAnswerDetail
                                {
                                    AnswerId = "a2",
                                    Text = "Berlin",
                                    IsCorrect = false,
                                    IsSelected = false
                                }
                            },
                            {
                                "a3",
                                new SingleChoiceAnswerDetail
                                {
                                    AnswerId = "a3",
                                    Text = "Madrid",
                                    IsCorrect = false,
                                    IsSelected = false
                                }
                            }
                        }
                    }
                },
                {
                    "q2",
                    new TestQuestionResult
                    {
                        QuestionId = "q2",
                        Text = "Which number is prime?",
                        IsCorrect = false,
                        AchievedPoints = 2,
                        TotalPossiblePoints = 5,
                        Type = "single_choice",

                        AnswerDetails = new Dictionary<string, AnswerDetail>
                        {
                            {
                                "a10",
                                new SingleChoiceAnswerDetail
                                {
                                    AnswerId = "a10",
                                    Text = "4",
                                    IsCorrect = false,
                                    IsSelected = true
                                }
                            },
                            {
                                "a11",
                                new SingleChoiceAnswerDetail
                                {
                                    AnswerId = "a11",
                                    Text = "5",
                                    IsCorrect = true,
                                    IsSelected = false
                                }
                            },
                        }
                    }
                }
            }
        };
        return Ok(test);
    }
}
