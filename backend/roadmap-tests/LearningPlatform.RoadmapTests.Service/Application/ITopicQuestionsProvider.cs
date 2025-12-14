using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using TopicQuestionsDto = LearningPlatform.RoadmapTests.Service.Application.Models.TopicQuestionsDto;

namespace LearningPlatform.RoadmapTests.Service.Application;

public interface ITopicQuestionsProvider
{
    Task<TopicQuestionsDto> GenerateTopicQuestions(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct);
}
