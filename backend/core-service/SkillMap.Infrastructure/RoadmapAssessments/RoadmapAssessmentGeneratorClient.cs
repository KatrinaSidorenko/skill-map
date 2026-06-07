using System.Net.Http.Json;

using Castle.Core.Logging;

using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly;

namespace SkillMap.Infrastructure.RoadmapAssessments;

public sealed class GenerateTopicQuestionsHttpRequest
{
    public Topic Topic { get; init; }
    public TopicQuestionsSettingDto Settings { get; init; }
}

public sealed class RoadmapAssessmentGeneratorClient : IQuestionsGenerator
{
    public const string ResiliencePipelineKey = "RoadmapAssessmentGenerator";
    private readonly HttpClient _httpClient;
    private readonly ILogger<IQuestionsGenerator> _logger;
    private readonly ResiliencePipeline _resiliencePipeline;

    public RoadmapAssessmentGeneratorClient(HttpClient httpClient, ILogger<IQuestionsGenerator> logger, IServiceProvider serviceProvider)
    {
        _httpClient = httpClient;
        _logger = logger;
        _resiliencePipeline = serviceProvider.GetRequiredKeyedService<ResiliencePipeline>(ResiliencePipelineKey);
    }

    public async Task<TopicQuestionsDto?> GenerateQuestionsForTopic(Topic topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        try
        {
            var request = new GenerateTopicQuestionsHttpRequest
            {
                Topic = topic,
                Settings = settings
            };

            using var response = await _httpClient.PostAsJsonAsync("api/topicquestions/generate", request, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TopicQuestionsDto>(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating questions for topic {TopicId}", topic.Id);
            return null;
        }
    }
    private async Task<(Topic Topic, TopicQuestionsDto? Questions)> GenerateQuestionsForTopic((Topic Topic, TopicQuestionsSettingDto Setting) topicWithSetting, CancellationToken ct)
    {
        var questions = await GenerateQuestionsForTopic(topicWithSetting.Topic, topicWithSetting.Setting, ct);
        return (topicWithSetting.Topic, questions);
    }
    private async ValueTask<(Topic Topic, TopicQuestionsDto? Questions)> GenerateQuestionsForTopicValueTask((Topic Topic, TopicQuestionsSettingDto Setting) topicWithSetting, CancellationToken ct)
        => await GenerateQuestionsForTopic(topicWithSetting, ct);
    private async Task<(Topic Topic, TopicQuestionsDto? Questions)> GenerateQuestionsForTopicWithRetry((Topic Topic, TopicQuestionsSettingDto Setting) topicWithSetting, CancellationToken ct)
        => await _resiliencePipeline.ExecuteAsync<(Topic Topic, TopicQuestionsDto? Questions)>((ct) => GenerateQuestionsForTopicValueTask(topicWithSetting, ct));

    public async Task<List<TopicQuestionsDto>> GenerateQuestionsForTopics(List<(Topic Topic, TopicQuestionsSettingDto Setting)> topicsWithGenerationSetting, CancellationToken ct)
    {
        var tasks = topicsWithGenerationSetting.Select(ts => GenerateQuestionsForTopicWithRetry(ts, ct));
        var generatedTopicQuestions = await Task.WhenAll(tasks);
        var failedTopicQuestionGeneration = generatedTopicQuestions.Where(r => r.Questions == null).ToList();
        return generatedTopicQuestions.Where(r => r.Questions != null).Select(r => r.Questions!).ToList();
    }
}