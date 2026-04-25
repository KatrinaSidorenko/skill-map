using System.Text.Json.Serialization;
using SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;

namespace SkillMap.Api.RoadmapAssessments.GetRoadmapStateSuggestions;

public class RoadmapStateSuggestionsResponse
{
    [JsonPropertyName("suggestedItems")]
    public Dictionary<string, List<RoadmapStateSuggestionItemResponse>> SuggestedItemsByTopic { get; init; }
    [JsonPropertyName("topics")]
    public List<RoadmapStateSuggestionTopicResponse> Topics { get; init; }
    public static RoadmapStateSuggestionsResponse Create(RoadmapStateSuggestionsDto dto) => new()
    {
        SuggestedItemsByTopic = dto.SuggestionsByTopics.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.Select(RoadmapStateSuggestionItemResponse.Create).ToList()),
        Topics = dto.Topics.Select(RoadmapStateSuggestionTopicResponse.Create).ToList()
    };
}

public class RoadmapStateSuggestionTopicResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    [JsonPropertyName("title")]
    public string Title { get; init; }
    public static RoadmapStateSuggestionTopicResponse Create(TopicDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title
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