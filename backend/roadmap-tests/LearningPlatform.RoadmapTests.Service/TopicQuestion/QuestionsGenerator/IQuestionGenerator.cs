using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using QuestionDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator;

public interface IQuestionGenerator
{
    Task<List<QuestionDto>> Generate(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct);
}
