using System.Text.Json.Serialization;

using SkillMap.Business.RoadmapAssessments.Features.GetRoadmapAssessment;

namespace SkillMap.Api.RoadmapAssessments.GetRoadmapAssessment;

public class RoadmapAssessmentResponse
{
    [JsonPropertyName("assessmentId")]
    public long AssessmentId { get; init; }

    [JsonPropertyName("questions")]
    public List<RoadmapAssessmentQuestionResponse> Questions { get; init; }

    public static RoadmapAssessmentResponse Create(RoadmapAssessmentDto dto) =>
        new()
        {
            AssessmentId = dto.AssessmentId,
            Questions = dto.Questions
                .Select(RoadmapAssessmentQuestionResponse.Create)
                .ToList()
        };
}

public class RoadmapAssessmentQuestionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("text")]
    public string Text { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("answers")]
    public List<RoadmapAssessmentAnswerResponse> Answers { get; init; }

    public static RoadmapAssessmentQuestionResponse Create(RoadmapAssessmentQuestionDto dto) =>
        new()
        {
            Id = dto.Id,
            Text = dto.Text,
            Type = dto.Type,
            Answers = dto.Answers
                .Select(RoadmapAssessmentAnswerResponse.Create)
                .ToList()
        };
}

public class RoadmapAssessmentAnswerResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("text")]
    public string Text { get; init; }

    public static RoadmapAssessmentAnswerResponse Create(RoadmapAssessmentAnswerDto dto) =>
        new()
        {
            Id = dto.Id,
            Text = dto.Text
        };
}