using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.OpenAi.Prompts;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator.OpenAi.Validators;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using SkillMap.Shared.Options;
using AnswerDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.AnswerDto;
using QuestionDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator;

public interface IOpenAiQuestionGenerator : IQuestionGenerator { }

public sealed class OpenAiQuestionGenerator : IOpenAiQuestionGenerator
{
    private readonly OpenAIClient _client;
    private readonly OpenAiOptions _options;
    private readonly IQuestionResponseValidator _validator;

    public OpenAiQuestionGenerator(
        OpenAIClient client,
        IOptions<OpenAiOptions> options,
        IQuestionResponseValidator validator)
    {
        _client = client;
        _options = options.Value;
        _validator = validator;
    }

    public async Task<List<QuestionDto>> Generate(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct)
    {
        var (systemPrompt, userPrompt) = PromptBuilder.Build(topic, settings);

        var chatClient = _client.GetChatClient(_options.Model);
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

        if (!string.IsNullOrEmpty(response.Value.Refusal))
        {
            throw new InvalidOperationException($"OpenAI refused to generate: {response.Value.Refusal}");
        }

        var finishReason = response.Value.FinishReason;
        if (finishReason == ChatFinishReason.ContentFilter)
        {
            throw new InvalidOperationException("Content was filtered by Azure/OpenAI safety systems.");
        }
        if (finishReason == ChatFinishReason.Length)
        {
            throw new InvalidOperationException("Output token limit reached before JSON was complete.");
        }

        var content = response.Value.Content
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Text))
            ?.Text;

        if (string.IsNullOrWhiteSpace(content))
        {
            // Log the full response for debugging
            var debugInfo = System.Text.Json.JsonSerializer.Serialize(response.Value);
            throw new InvalidOperationException(
                $"OpenAI returned empty content. FinishReason: {finishReason}. Full Response: {debugInfo}");
        }


        // 1️⃣ Parse JSON
        var parsed = JsonConvert.DeserializeObject<OpenAiQuestionsResponse>(content)
                     ?? throw new InvalidOperationException("Invalid JSON from OpenAI");

        // 2️⃣ Validate semantic correctness
        _validator.Validate(parsed, settings);
        var validQuestions = parsed.Questions
            .Where(q => _validator.IsValidQuestion(q, settings))
            .ToList();

        // 3️⃣ Map to DTOs
        return MapToDto(validQuestions, topic);
    }

    private static List<QuestionDto> MapToDto(
        List<OpenAiQuestion> openAiQuestions,
        TopicDto topic)
    {
        return openAiQuestions.Select((q, qi) => new QuestionDto
        {
            Id = $"{topic.Id}-q{qi + 1}",
            Text = q.Text,
            Type = TestQuestionType.SingleChoice,
            Answers = q.Answers.Select((a, ai) => new AnswerDto
            {
                Id = $"{topic.Id}-q{qi + 1}-a{ai + 1}",
                Text = a.Text,
                IsCorrect = a.IsCorrect
            }).ToList()
        }).ToList();
    }
}
