namespace SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;
public record RoadmapStateSuggestionsDto(Dictionary<string, List<LearningItemSuggestionDto>> SuggestionsByTopics, List<TopicDto> Topics);
public record LearningItemSuggestionDto(string Id, string Title, string Type, string ActualStatus, string SuggestedStatus);

public record TopicDto(string Id, string Title);
