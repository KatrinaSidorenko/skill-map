using System.Text.Json.Serialization;

namespace SkillMap.Api.RoadmapAssessments.EvaluateRoadmapAssessment;

public class EvaluateRoadmapAssessmentRequest
{
    [JsonPropertyName("answers")]
    public List<RoadmapAssessmentProvidedAnswerRequest> ProvidedAnswers { get; set; }
}

public class RoadmapAssessmentProvidedAnswerRequest
{
    [JsonPropertyName("questionId")]
    public string QuestionId { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("selectedAnswerId")]
    public string? SelectedAnswerId { get; set; }
}