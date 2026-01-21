using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest;

public interface IRoadmapTestService
{
    Task<Result<RoadmapTestResultDto>> CreateInitialRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct);
    Task<string> EstimateRoadmapTest(string roadmapTestId, RoadmapTestAnswers userAnswers, CancellationToken ct);
    Task<RoadmapChangesSuggestionsDto> GetRoadmapChangesSuggestions(long userId, string roadmapTestResultId, CancellationToken ct);
    Task<Result<RoadmapTestResultDto>> CreateIntermediateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct);
}
