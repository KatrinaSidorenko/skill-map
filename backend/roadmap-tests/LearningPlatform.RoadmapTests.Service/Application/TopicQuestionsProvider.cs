using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Persistence.Abstractions;

using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

using QuestionDto = LearningPlatform.RoadmapTests.Service.Application.Models.QuestionDto;
using TopicQuestionsDto = LearningPlatform.RoadmapTests.Service.Application.Models.TopicQuestionsDto;

namespace LearningPlatform.RoadmapTests.Service.Application;

public sealed class TopicQuestionsProvider : ITopicQuestionsProvider
{
    private readonly IQuestionSource _generator;
    private readonly ITopicQuestionsRepository _topicQuestionsRepository;
    public TopicQuestionsProvider(IQuestionSource generator, ITopicQuestionsRepository topicQuestionsRepository)
    {
        _generator = generator;
        _topicQuestionsRepository = topicQuestionsRepository;
    }

    // todo: create more clever logic. Long polling system.
    public async Task<TopicQuestionsDto> GenerateTopicQuestions(TopicDto topic,  TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (settings.QuestionsCount <= 0)
            throw new ArgumentException("QuestionsCount must be > 0");

        if (settings.Types.Count == 0)
            throw new ArgumentException("At least one question type required");

        var questions = await _generator.GetUniqueQuestionsForTopic(topic, settings, ct);
        if (!questions.IsSuccessful || !questions.HasData)
        {
            throw new InvalidOperationException(
                $"Failed to generate questions for topic {topic.Id}. Reason: {questions.Reason}");
        }

        return new TopicQuestionsDto
        {
            Id = topic.Id,
            Questions = questions.Data,
        };
    }
}