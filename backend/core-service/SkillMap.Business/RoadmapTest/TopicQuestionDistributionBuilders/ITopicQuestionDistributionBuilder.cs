using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicQuestionDistributionBuilder;

namespace SkillMap.Business.RoadmapTest.TopicAnalyzers;
public interface ITopicQuestionDistributionBuilder
{
    Dictionary<string, TopicQuestionPlan> BuildTopicQuestionDistribution(List<Topic> topics, RoadmapTestConfigDto config, CancellationToken ct);
}