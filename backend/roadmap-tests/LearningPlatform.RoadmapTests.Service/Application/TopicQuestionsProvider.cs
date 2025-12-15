using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using SkillMap.Shared.Results;
using TopicQuestionsDto = LearningPlatform.RoadmapTests.Service.Application.Models.TopicQuestionsDto;

namespace LearningPlatform.RoadmapTests.Service.Application;

public sealed class TopicQuestionsProvider : ITopicQuestionsProvider
{
    private readonly IQuestionSource _generator;

    public TopicQuestionsProvider(
        IQuestionSource generator)
    {
        _generator = generator;
    }

    // todo: create more clever logic. Long polling system.
    public async Task<TopicQuestionsDto> GenerateTopicQuestions(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct)
    {
        // here we should work with domain models?
        ct.ThrowIfCancellationRequested();

        if (settings.QuestionsCount <= 0)
            throw new ArgumentException("QuestionsCount must be > 0");

        if (settings.Types.Count == 0)
            throw new ArgumentException("At least one question type required");

        var questions = await _generator.Generate(topic, settings, ct);
        if (!questions.IsSuccessful || !questions.HasData)
        {
            throw new InvalidOperationException(
                $"Failed to generate questions for topic {topic.Id}. Reason: {questions.Reason}");
        }

        // add responsibility of caching + storing into database

        return new TopicQuestionsDto
        {
            Id = topic.Id,
            Questions = questions.Data,
        };
    }
}
