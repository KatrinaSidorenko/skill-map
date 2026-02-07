using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest.TopicQuestionComposers;

public interface ITopicQuestionComposer
{
    Task<Result<RoadmapTestResult>> GenerateRoadmapTestQuestions(List<Node> nodes, List<Edge> edges, RoadmapTestConfigDto config, CancellationToken ct);
}