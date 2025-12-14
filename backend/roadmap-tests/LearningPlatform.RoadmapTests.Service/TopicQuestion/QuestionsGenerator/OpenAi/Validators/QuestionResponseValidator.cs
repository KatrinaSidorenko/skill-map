using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.Common;
using SkillMap.Shared.Results;
using System.Runtime.CompilerServices;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.OpenAi.Validators;

public static class QuestionResponseValidator
{
    public static bool IsValidQuestion(this OpenAiQuestion openAiQuestion, TopicQuestionsSettingDto settings)
    {
         if (string.IsNullOrWhiteSpace(openAiQuestion.Text))
            return false;

        if (!openAiQuestion.Type.IsAvailableQuestionType())
            return false;

        if (!settings.TypeStrings.Contains(openAiQuestion.Type))
            return false;

        return true;
    }
}
