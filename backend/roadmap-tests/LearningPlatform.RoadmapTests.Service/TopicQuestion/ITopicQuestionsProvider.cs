using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using TopicQuestionsDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.TopicQuestionsDto;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion;

public interface ITopicQuestionsProvider
{
    Task<TopicQuestionsDto> GenerateTopicQuestions(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct);
}
