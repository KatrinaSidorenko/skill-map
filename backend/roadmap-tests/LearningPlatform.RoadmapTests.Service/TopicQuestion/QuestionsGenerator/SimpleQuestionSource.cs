using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.Common;
using AnswerDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.AnswerDto;
using QuestionDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator;

public interface ISimpleQuestionSource : IQuestionSource { }
public sealed class SimpleQuestionSource : ISimpleQuestionSource
{
    public async Task<GenerationResult<List<QuestionDto>>> Generate(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct)
    {
        await Task.Delay(50, ct); // simulate I/O

        var questions = new List<QuestionDto>();
        var random = new Random();

        for (int i = 0; i < settings.QuestionsCount; i++)
        {
            var type = settings.Types[i % settings.Types.Count];
            var questionId = $"{topic.Id}-q{i + 1}";

            questions.Add(new QuestionDto
            {
                Id = questionId,
                Type = type,
                Text = $"[{settings.DifficultyLevel}] What is {topic.Name}?",
                Answers = GenerateAnswers(questionId)
            });
        }

        return new GenerationResult<List<QuestionDto>>(questions);
    }

    private static List<AnswerDto> GenerateAnswers(string questionId)
    {
        return new List<AnswerDto>
    {
        new()
        {
            Id = $"{questionId}-a1",
            Text = "Correct answer",
            IsCorrect = true
        },
        new()
        {
            Id = $"{questionId}-a2",
            Text = "Incorrect answer",
            IsCorrect = false
        },
        new()
        {
            Id = $"{questionId}-a3",
            Text = "Another incorrect answer",
            IsCorrect = false
        }
    };
    }
}
