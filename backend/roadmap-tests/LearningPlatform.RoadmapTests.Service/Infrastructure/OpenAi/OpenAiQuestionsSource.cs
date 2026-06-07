using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;
using LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi.Prompts;
using LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi.Validators;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using OpenAI;
using OpenAI.Chat;

using Polly;

using SkillMap.Shared.Options;

using AnswerDto = LearningPlatform.RoadmapTests.Service.Application.Models.AnswerDto;
using QuestionDto = LearningPlatform.RoadmapTests.Service.Application.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi;

public sealed class OpenAiQuestionsSource : IOpenAiQuestionSource
{
    public const string ResiliencePipelineKey = "OpenAiClient";
    private readonly OpenAIClient _client;
    private readonly IOptions<OpenAiOptions> _options;
    private readonly ResiliencePipeline _resiliencePipeline;
    public OpenAiQuestionsSource(OpenAIClient client, IOptions<OpenAiOptions> options, IServiceProvider serviceProvider)
    {
        _client = client;
        _options = options;
        _resiliencePipeline = serviceProvider.GetRequiredKeyedService<ResiliencePipeline>(ResiliencePipelineKey);
    }
  
    public async Task<GenerationResult<List<QuestionDto>>> GetUniqueQuestionsForTopic(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
            => await GetUniqueQuestionsForTopicWithRetry(topic, settings, ct);
    private async Task<GenerationResult<List<QuestionDto>>> GetUniqueQuestionsForTopicInner(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(topic);
        ArgumentNullException.ThrowIfNull(settings);

        var (systemPrompt, userPrompt) = PromptBuilder.Build(topic, settings);
        var chatClient = _client.GetChatClient(_options.Value.Model);
        var response = await chatClient.CompleteChatAsync(
            messages: new[]
            {
                ChatMessage.CreateSystemMessage(systemPrompt),
                ChatMessage.CreateUserMessage(userPrompt) as ChatMessage
            },
            options: new ChatCompletionOptions
            {
                //MaxOutputTokenCount = settings.QuestionsCount * (_options.MaxOutputTokens ?? 800), // todo: create clever logic for token count
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
            },
            cancellationToken: ct
        );
        var initialResponseValidation = response.ValidateResponse<List<QuestionDto>>();
        if (initialResponseValidation is not null && !initialResponseValidation.IsSuccessful) { return initialResponseValidation; }

        var contentValidation = response.ValidateResponseContent();
        if (contentValidation is not null && !contentValidation.IsSuccessful) { return contentValidation.ToGenerationResult<List<QuestionDto>, OpenAiQuestionsResponse>(); }

        var generatedQuestions = contentValidation.Data.Questions.Where(q => q.IsValidQuestion(settings)).ToList();
        return new GenerationResult<List<QuestionDto>>(generatedQuestions.Select(q => q.ToQuestionDto()).ToList());
    }
    private async ValueTask<GenerationResult<List<QuestionDto>>> GetUniqueQuestionsForTopicValueTask(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
        => await GetUniqueQuestionsForTopicInner(topic, settings, ct);
    private async Task<GenerationResult<List<QuestionDto>>> GetUniqueQuestionsForTopicWithRetry(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
        => await _resiliencePipeline.ExecuteAsync(ct => GetUniqueQuestionsForTopicValueTask(topic, settings, ct));

}