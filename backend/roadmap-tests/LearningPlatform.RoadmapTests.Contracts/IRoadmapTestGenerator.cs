using LearningPlatform.RoadmapTests.Contracts.Models;

namespace LearningPlatform.RoadmapTests.Contracts;

public interface IRoadmapTestGenerator
{
    Task<TopicQuestionsDto> GenerateTopicQuestions(Topic topic, TopicQuestionsSettingDto settings, CancellationToken ct);
    Task<List<TopicQuestionsDto>> GenerateRoadmapTest(List<(Topic topic, TopicQuestionsSettingDto settings)> topicsSettings, CancellationToken ct);
}
