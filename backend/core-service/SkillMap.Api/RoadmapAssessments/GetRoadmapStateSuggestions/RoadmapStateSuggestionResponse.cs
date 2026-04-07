using System.Text.Json.Serialization;
using SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;

namespace SkillMap.Api.RoadmapAssessments.GetRoadmapStateSuggestions;

public class RoadmapStateSuggestionsResponse
{
    [JsonPropertyName("suggestedItems")]
    public List<RoadmapStateSuggestionItemResponse> SuggestedItems { get; init; }
    [JsonPropertyName("topicToSubtopicConnections")]
    public Dictionary<string, List<string>> TopicToSubtopicConnections { get; init; }

    public static RoadmapStateSuggestionsResponse Create(RoadmapStateSuggestionsDto dto) => new()
    {
        SuggestedItems = dto.SuggestedItems
            .Select(RoadmapStateSuggestionItemResponse.Create)
            .ToList(),
        TopicToSubtopicConnections = dto.TopicToSubtopicConnections
    };
}

public class RoadmapStateSuggestionItemResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("actualStatus")]
    public string ActualStatus { get; init; }

    [JsonPropertyName("suggestedStatus")]
    public string SuggestedStatus { get; init; }

    public static RoadmapStateSuggestionItemResponse Create(LearningItemSuggestionDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Type = dto.Type,
        ActualStatus = dto.ActualStatus,
        SuggestedStatus = dto.SuggestedStatus
    };
}