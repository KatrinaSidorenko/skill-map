namespace SkillMap.Business.RoadmapAssessments.Features.GetRoadmapStateSuggestions;
public record RoadmapStateSuggestionsDto(List<LearningItemSuggestionDto> SuggestedItems);
public record LearningItemSuggestionDto(string Id, string Title, string Type, string ActualStatus, string SuggestedStatus);
