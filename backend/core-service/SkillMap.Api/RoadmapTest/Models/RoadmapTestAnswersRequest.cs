using LearningPlatform.RoadmapTests.Contracts;
using Newtonsoft.Json;
using SkillMap.Business.RoadmapTest.Models;

namespace SkillMap.Api.RoadmapTest.Models;

public class RoadmapTestAnswersRequest
{
    [JsonProperty("questionAnswers")]
    public List<QuestionAnswer> QuestionAnswers { get; set; }

    public RoadmapTestAnswers ToDto()
    {
        return new RoadmapTestAnswers
        {
            QuestionAnswers = this.QuestionAnswers.Select(qa => qa.ToDto()).ToList()
        };
    }
}

public class QuestionAnswer
{
    [JsonProperty("questionId")]
    public string QuestionId { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("selectedAnswerId")]
    public string? SelectedAnswerId { get; set; }
    public SkillMap.Business.RoadmapTest.Models.QuestionAnswer ToDto()
    {
        return Type.FromQuestionTypeString() switch
        {
            TestQuestionType.SingleChoice => new SkillMap.Business.RoadmapTest.Models.SingleChoiceAnswer
            {
                QuestionId = this.QuestionId,
                SelectedAnswerId = this.SelectedAnswerId
            },
            _ => throw new NotSupportedException($"Unsupported question answer type: {Type}")
        };
    }
}