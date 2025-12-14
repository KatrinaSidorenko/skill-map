using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.OpenAi.Validators;

public interface IQuestionResponseValidator
{
    bool IsValidQuestion(OpenAiQuestion openAiQuestion, TopicQuestionsSettingDto settings);
    void Validate(OpenAiQuestionsResponse response, TopicQuestionsSettingDto settings);
}
