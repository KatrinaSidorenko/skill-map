using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Shared.Results;
using TopicQuestionsDto = LearningPlatform.RoadmapTests.Contracts.Models.TopicQuestionsDto;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion;

[Route("api/[controller]")]
[ApiController]
public class TopicQuestionsController : ControllerBase
{
    private readonly ITopicQuestionsProvider _service;

    public TopicQuestionsController(
        ITopicQuestionsProvider service)
    {
        _service = service;
    }


    [HttpPost("generate")]
    public async Task<ActionResult<TopicQuestionsDto>> Generate(
        [FromBody] GenerateTopicQuestionsRequest request,
        CancellationToken ct)
    {
        var validation = request.Validate();
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var result = await _service.GenerateTopicQuestions(
            request.Topic,
            request.Settings,
            ct);

        return Ok(result);
    }
}
