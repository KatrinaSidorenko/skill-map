using LearningPlatform.RoadmapTests.Contracts.Models;

namespace LearningPlatform.RoadmapTests.Contracts;

public interface ITopicQuestionsGenerator
{
    Task<TopicQuestionsDto> GenerateTopicQuestions(Topic topic, TopicQuestionsSettingDto settings, CancellationToken ct);
    Task<List<TopicQuestionsDto>> GenerateTopicsQuestions(List<(Topic topic, TopicQuestionsSettingDto settings)> topicsSettings, CancellationToken ct);
}