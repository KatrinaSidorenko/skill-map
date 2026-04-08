namespace SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;
public record RoadmapStateSuggestionsDto(List<LearningItemSuggestionDto> SuggestedItems, Dictionary<string, List<string>> TopicToSubtopicConnections);
public record LearningItemSuggestionDto(string Id, string Title, string Type, string ActualStatus, string SuggestedStatus);
