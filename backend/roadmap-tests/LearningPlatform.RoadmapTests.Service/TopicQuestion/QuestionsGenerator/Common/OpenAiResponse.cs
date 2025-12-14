using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using SkillMap.Shared.Extensions;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.Common;

public sealed class OpenAiQuestionsResponse
{
    public List<OpenAiQuestion> Questions { get; set; }
}

public sealed class OpenAiQuestion
{
    public string Text { get; set; }
    public string Type { get; set; }
    public List<OpenAiAnswer> Answers { get; set; }

    public QuestionDto ToQuestionDto()
    {
        return new QuestionDto
        {
            Id = Guid.NewGuid().WithoutHyphens(),
            Text = Text,
            Type = Type.FromQuestionTypeString(),
            Answers = Answers.Select(a => a.ToAnswerDto()).ToList()
        };
    }
}

public sealed class OpenAiAnswer
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }

    public AnswerDto ToAnswerDto()
    {
        return new AnswerDto
        {
            Id = Guid.NewGuid().WithoutHyphens(),
            Text = Text,
            IsCorrect = IsCorrect
        };
    }
}
