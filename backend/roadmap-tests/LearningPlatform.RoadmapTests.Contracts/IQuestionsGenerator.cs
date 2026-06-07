using LearningPlatform.RoadmapTests.Contracts.Models;

namespace LearningPlatform.RoadmapTests.Contracts;

public interface IQuestionsGenerator
{
    Task<TopicQuestionsDto?> GenerateQuestionsForTopic(Topic topic, TopicQuestionsSettingDto settings, CancellationToken ct);
    Task<List<TopicQuestionsDto>> GenerateQuestionsForTopics(List<(Topic Topic, TopicQuestionsSettingDto Setting)> topicsWithGenerationSetting, CancellationToken ct);
}