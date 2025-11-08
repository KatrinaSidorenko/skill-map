using LearningPlatform.RoadmapTests.Contracts.Models;

namespace LearningPlatform.RoadmapTests.Contracts;

public interface IRoadmapTestGenerator
{
    Task<TopicQuestionsDto> GenerateTopicQuestions(Topic topic, TopicQuestionSetting settings, CancellationToken ct);
    Task<List<TopicQuestionsDto>> GenerateRoadmapTest(List<(Topic topic, TopicQuestionSetting settings)> topicsSettings, CancellationToken ct);
}
