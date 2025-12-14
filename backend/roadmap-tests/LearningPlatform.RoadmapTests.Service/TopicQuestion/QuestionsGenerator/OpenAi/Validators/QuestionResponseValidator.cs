using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using SkillMap.Shared.Results;
using System.Runtime.CompilerServices;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.OpenAi.Validators;

public sealed class QuestionResponseValidator : IQuestionResponseValidator
{
    public void Validate(OpenAiQuestionsResponse response, TopicQuestionsSettingDto settings)
    {
        if (response.Questions.Count != settings.QuestionsCount)
            throw new LearningPlatformException(
                ErrorCode.VALIDATION_ERROR,
                $"Incorrect question count. Expected: {settings.QuestionsCount}, Actual: {response.Questions.Count}");
    }

    public bool IsValidQuestion(OpenAiQuestion openAiQuestion, TopicQuestionsSettingDto settings)
    {
         if (string.IsNullOrWhiteSpace(openAiQuestion.Text))
            return false;

         if (!settings.TypeStrings.Contains(openAiQuestion.Type))
            return false;

         return true;
    }
}
