using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Business.RoadmapTest.Models;

namespace SkillMap.Business.RoadmapTest;

public interface IRoadmapTestService
{
    Task<RoadmapTestResultDto> CreateInitialRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct);
    Task<SavedUerRoadmap> RebuildRoadmapBasedOnTestResults(long userId, string roadmapId, CancellationToken ct);
    Task<string> EstimateRoadmapTest(string roadmapTestId, RoadmapTestAnswers userAnswers, CancellationToken ct);
    Task<RoadmapChangesSuggestionsDto> GetRoadmapChangesSuggestions(long userId, string roadmapTestResultId, CancellationToken ct);
}
