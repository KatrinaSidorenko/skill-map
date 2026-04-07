using System.Text.Json.Serialization;

using SkillMap.Business.RoadmapAssessments.Features.ApplyRoadmapStateSuggestions;

namespace SkillMap.Api.RoadmapAssessments.ApplyRoadmapStateSuggestions;

public class ApplyRoadmapStateSuggestionsRequest
{
    [JsonPropertyName("items")]
    public List<ApplySuggestionItemRequest> Items { get; init; } = [];

    public ApplyRoadmapStateSuggestionsCommand ToCommand(long attemptId) =>
        new(attemptId, Items.Select(i => new SuggestionItemCommand(i.Id, i.Type, i.Status)).ToList());
}

public class ApplySuggestionItemRequest
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; }
}
