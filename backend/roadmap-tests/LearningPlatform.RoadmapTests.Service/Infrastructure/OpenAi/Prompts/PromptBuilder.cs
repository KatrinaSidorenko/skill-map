using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using System.Text;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi.Prompts;

public class PromptBuilder
{
    public static (string system, string user) Build(
       TopicDto topic,
       TopicQuestionsSettingDto settings)
    {
        var system = PromptTemplates.SystemInstructions;

        var user = PromptTemplates.TopicContext
            .Replace("{TOPIC_NAME}", Sanitize(topic.Name))
            .Replace("{TOPIC_DESCRIPTION}", Sanitize(topic.Description))
            .Replace("{DIFFICULTY}", settings.DifficultyLevel.ToDifficultyString())
            .Replace("{QUESTIONS_COUNT}", settings.QuestionsCount.ToString())
            .Replace("{QUESTION_TYPES}", string.Join(", ", settings.TypeStrings));

        return (system, user);
    }

    /// <summary>
    /// Prevents prompt injection & malformed content
    /// </summary>
    private static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return input
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Trim();
    }
}
