using System.Text.Json.Serialization;

namespace SkillMap.Api.RoadmapAssessments.CreateAssessmentAttempt;

public class CreateAssessmentAttemptResponse
{
    [JsonPropertyName("attemptId")]
    public long AttemptId { get; init; }

    public static CreateAssessmentAttemptResponse Create(long attemptId) => new() { AttemptId = attemptId };
}