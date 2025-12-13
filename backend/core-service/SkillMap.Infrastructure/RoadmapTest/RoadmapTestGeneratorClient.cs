using System.Net.Http.Json;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

namespace SkillMap.Infrastructure.RoadmapTest;

public sealed class GenerateTopicQuestionsHttpRequest
{
    public Topic Topic { get; init; }
    public TopicQuestionsSettingDto Settings { get; init; }
}

public sealed class RoadmapTestGeneratorClient : IRoadmapTestGenerator
{
    private readonly HttpClient _httpClient;

    public RoadmapTestGeneratorClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TopicQuestionsDto> GenerateTopicQuestions(
        Topic topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct)
    {
        var request = new GenerateTopicQuestionsHttpRequest
        {
            Topic = topic,
            Settings = settings
        };

        using var response = await _httpClient.PostAsJsonAsync("api/topicquestions/generate", request, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TopicQuestionsDto>(cancellationToken: ct);

        if (result is null)
            throw new InvalidOperationException(
                "Topic questions service returned empty response");

        return result;
    }

    public async Task<List<TopicQuestionsDto>> GenerateRoadmapTest(
        List<(Topic topic, TopicQuestionsSettingDto settings)> topicsSettings,
        CancellationToken ct)
    {
        // Parallelize topic calls (VERY important)
        var tasks = topicsSettings.Select(ts =>
            GenerateTopicQuestions(ts.topic, ts.settings, ct));

        var results = await Task.WhenAll(tasks);

        return results.ToList();
    }
}
