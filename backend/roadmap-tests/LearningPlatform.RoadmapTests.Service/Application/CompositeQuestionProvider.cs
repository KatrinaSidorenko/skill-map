using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;

using Microsoft.Extensions.Options;

using Polly;
using Polly.Fallback;

using SkillMap.Shared.Extensions;

using QuestionDto = LearningPlatform.RoadmapTests.Service.Application.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.Application;

public sealed class CompositeQuestionProvider : IQuestionSource
{
    private readonly List<IQuestionSource> _pipeline;
    private readonly ILogger<CompositeQuestionProvider> _logger;


    public CompositeQuestionProvider(
        ILogger<CompositeQuestionProvider> logger,
        ICacheQuestionSource cacheGenerator,
        IDatabaseQuestionSource databaseGenerator,
        IOpenAiQuestionSource openAiGenerator)
    {
        _logger = logger;

        _pipeline = new List<IQuestionSource>
        {
            cacheGenerator,     // 1. Check Memory (Instant)
            //databaseGenerator,  // 2. Check SQL (Fast)
            openAiGenerator,    // 3. Generate New (Slow, $$)
        };
    }

  

    public async Task<GenerationResult<List<QuestionDto>>> GetUniqueQuestionsForTopic(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
        => await GenerateWithIdentifiers(topic, settings, ct);
    private async Task<GenerationResult<List<QuestionDto>>> InnerGenerate(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        var finalQuestions = new List<QuestionDto>();
        var uniqueTracker = new HashSet<int>();
        var targetQuestionsAmount = settings.QuestionsCount;

        foreach (var generator in _pipeline)
        {
            ct.ThrowIfCancellationRequested();

            var needed = targetQuestionsAmount - finalQuestions.Count;
            if (needed <= 0) { break; }

            var stepSettings = settings.DeepCopy();
            stepSettings.QuestionsCount = needed;

            try
            {
                var result = await generator.GetUniqueQuestionsForTopic(topic, stepSettings, ct);

                if (!result.IsSuccessful || !result.HasData)
                {
                    _logger.LogWarning(
                         "Generator {Type} failed with reason: {Reason}",
                         generator.GetType().Name, result.Reason?.ToString() ?? "Unknown");
                    continue;
                }

                finalQuestions.AddRange(result.Data.Where(q => uniqueTracker.Add(q.Text.GetHashCode())));

                _logger.LogInformation(
                    "Generator {Type} provided {Count} questions. Total: {Total}/{Target}",
                    generator.GetType().Name, result.Data.Count, finalQuestions.Count, settings.QuestionsCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generator {Type} failed to execute.", generator.GetType().Name);
            }
        }

        return new GenerationResult<List<QuestionDto>>(finalQuestions);
    }

    private async Task<GenerationResult<List<QuestionDto>>> GenerateWithIdentifiers(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct)
    {
        var result = await InnerGenerate(topic, settings, ct);
        if (!result.IsSuccessful || !result.HasData)
        {
            return new GenerationResult<List<QuestionDto>>(result.Reason);
        }
        var questionsWithIds = result.Data.Select(q =>
        {
            q.Id = Guid.NewGuid().ToStringWithoutHyphens();
            q.Answers.ForEach(a =>
            {
                a.Id = Guid.NewGuid().ToStringWithoutHyphens();
            });
            return q;
        }).ToList();
        return new GenerationResult<List<QuestionDto>>(questionsWithIds);
    }
}