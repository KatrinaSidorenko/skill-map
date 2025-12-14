using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using QuestionDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator;

public sealed class TopicQuestionsGenerator : IQuestionGenerator
{
    private readonly ISimpleQuestionGenerator _simpleGenerator;
    private readonly IOpenAiQuestionGenerator _openAiGenerator;
    private readonly ILogger<TopicQuestionsGenerator> _logger;

    public TopicQuestionsGenerator(
        ILogger<TopicQuestionsGenerator> logger,
        ISimpleQuestionGenerator simpleQuestionGenerator,
        IOpenAiQuestionGenerator openAiQuestionGenerator)
    {
        _logger = logger;
        _simpleGenerator = simpleQuestionGenerator;
        _openAiGenerator = openAiQuestionGenerator;
    }

    public async Task<List<QuestionDto>> Generate(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // 1️⃣ Try OpenAI first (best quality)
        try
        {
            var aiResult = await _openAiGenerator.Generate(topic, settings, ct);

            if (aiResult is { Count: > 0 })
                return aiResult;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI question generation failed, falling back");
            // TODO: add structured logging here
            // logger.LogWarning(ex, "OpenAI question generation failed, falling back");
        }

        // 2️⃣ Fallback to simple generator (guaranteed availability)
        var fallbackResult = await _simpleGenerator.Generate(topic, settings, ct);

        if (fallbackResult is { Count: > 0 })
            return fallbackResult;

        // 3️⃣ Absolute failure (should never happen)
        throw new InvalidOperationException(
            "Failed to generate questions using both OpenAI and fallback generator");
    }
}
