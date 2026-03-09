namespace SkillMap.Api.RoadmapTest.Models;

public class RoadmapTestSuggestionResponse
{
    public List<RoadmapTestSuggestionItem> Suggestions { get; set; } = new();
}

public class RoadmapTestSuggestionItem
{
    public string LearningItemId { get; set; } = string.Empty;
    public string LearningStatus { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}