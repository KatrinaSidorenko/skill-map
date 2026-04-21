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

        questions.Data.ForEach(q =>
        {
            q.Id = Guid.NewGuid().ToStringWithoutHyphens();
            q.Answers.ForEach(a =>
            {
                a.Id = Guid.NewGuid().ToStringWithoutHyphens();
            });
        });
        // todo: move to background job
        // todo: add categories to topic and questions
        //_ = ProcessGeneratedQuestions(
        //    topic,
        //    settings,
        //    questions.Data,
        //    CancellationToken.None);


        return new TopicQuestionsDto
        {
            Id = topic.Id,
            Questions = questions.Data,
        };
    }

    private async Task<(long? TopicId, List<QuestionDto> SavedQuestions)> ProcessGeneratedQuestions(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        IEnumerable<QuestionDto> questions,
        CancellationToken ct)
    {
        var difficultyLevel = settings.DifficultyLevel.ToDifficultyString();
        var topicEntity = new Persistence.Models.TopicEntity
        {
            ExternalId = topic.Id,
            Name = topic.Name,
            Description = topic.Description
        };
        var questionEntities = questions.Where(q => q.IsGenerated).Select(q => new Persistence.Models.QuestionEntity
        {
            ExternalId = q.Id,
            Text = q.Text,
            Difficulty = difficultyLevel,
            Type = q.Type.ToQuestionTypeString(),
            Answers = q.Answers.JsonSerializeOrDefault(),
        });
        if (!questionEntities.Any()) { return (null, null); }

        // todo: add check on duplications of topic by text and desctiption
        var topicId = await _topicQuestionsRepository.InsertTopicWithQuestions(topicEntity, questionEntities, ct);
        return (topicId, questions.ToList());
    }
}