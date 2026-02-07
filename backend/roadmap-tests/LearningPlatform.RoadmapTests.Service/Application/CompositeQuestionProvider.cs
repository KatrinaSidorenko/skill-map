using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;

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
        IOpenAiQuestionSource openAiGenerator,
        ISimpleQuestionSource simpleGenerator)
    {
        _logger = logger;

        _pipeline = new List<IQuestionSource>
        {
            cacheGenerator,     // 1. Check Memory (Instant)
            databaseGenerator,  // 2. Check SQL (Fast)
            openAiGenerator,    // 3. Generate New (Slow, $$)
            simpleGenerator     // 4. Last Resort (Free, Low Quality)
        };
    }

    public async Task<GenerationResult<List<QuestionDto>>> Generate(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct)
    {
        var finalQuestions = new List<QuestionDto>();
        var uniqueTracker = new HashSet<int>();

        foreach (var generator in _pipeline)
        {
            ct.ThrowIfCancellationRequested();

            var needed = settings.QuestionsCount - finalQuestions.Count;
            if (needed <= 0) { break; }

            var stepSettings = settings.DeepCopy();
            stepSettings.QuestionsCount = needed;

            try
            {
                var result = await generator.Generate(topic, stepSettings, ct);

                if (!result.IsSuccessful && !result.HasData)
                {
                    _logger.LogWarning(
                         "Generator {Type} failed with reason: {Reason}",
                         generator.GetType().Name, result.Reason?.ToString() ?? "Unknown");
                    continue;
                }

                foreach (var q in result.Data)
                {
                    if (finalQuestions.Count >= settings.QuestionsCount) break;

                    if (uniqueTracker.Add(q.Text.GetHashCode()))
                    {
                        finalQuestions.Add(q);
                    }
                }

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
}