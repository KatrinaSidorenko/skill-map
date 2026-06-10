using FluentValidation;

using LearningPlatform.RoadmapTests.Contracts.Models;

using Newtonsoft.Json;

using SkillMap.Shared.Results;

namespace LearningPlatform.RoadmapTests.Service.Application.Models;

public sealed class GenerateTopicQuestionsRequest
{
    [JsonProperty("topic")]
    public TopicDto Topic { get; init; }
    [JsonProperty("settings")]
    public TopicQuestionsSettingDto Settings { get; init; }
}

public sealed record TopicDto(
    string Id,
    string Name,
    string Description);


public static class GenerateTopicInputValidator
{
    public static ResponseInfo Validate(this GenerateTopicQuestionsRequest request)
    {
        var messages = new List<string>();
        if (request.Topic is null)
            messages.Add("Topic is required");
        if (request.Settings is null)
            messages.Add("Settings are required");

        return messages.Count > 0
            ? new ResponseInfo(ErrorCode.INVALIDINPUT, string.Join("; ", messages))
            : null;
    }
}