using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts.Models;

namespace SkillMap.Business.RoadmapTest.TopicSelectors;

public interface IRoadmapTopicsSelector
{
    List<Topic> SelectCoreTopics(
        List<Node> nodes,
        List<Edge> edges,
        int questionsLimit);
}