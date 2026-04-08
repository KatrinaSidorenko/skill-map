using System.ClientModel;

using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;

using Newtonsoft.Json;

using OpenAI.Chat;

using SkillMap.Shared.Extensions;

using QuestionDto = LearningPlatform.RoadmapTests.Service.Application.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi.Validators;

public static class OpenAiResponseValidator
{
    public static GenerationResult<T> ValidateResponse<T>(this ClientResult<ChatCompletion> response)
    {
        if (response == null)
        {
            return new GenerationResult<T>(GenerationErrorReasons.ExternalServiceFailure);
        }

        if (!string.IsNullOrEmpty(response.Value.Refusal))
        {
            return new GenerationResult<T>(GenerationErrorReasons.OpenAIRefusal);
        }

        var finishReason = response.Value.FinishReason;
        if (finishReason == ChatFinishReason.ContentFilter)
        {
            return new GenerationResult<T>(GenerationErrorReasons.FilteredByOpenAI);
        }
        if (finishReason == ChatFinishReason.Length)
        {
            return new GenerationResult<T>(GenerationErrorReasons.GenerationTimeout);
        }

        return null;
    }

    public static GenerationResult<OpenAiQuestionsResponse> ValidateResponseContent(this ClientResult<ChatCompletion> response)
    {
        var content = response.Value.Content.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Text))?.Text;
        if (string.IsNullOrWhiteSpace(content))
        {
            return new GenerationResult<OpenAiQuestionsResponse>(GenerationErrorReasons.OpenAIOutputIsEmpty(response.Value.JsonSerializeOrDefault()));
        }

        var parsed = content.JsonDeserializeOrDefault<OpenAiQuestionsResponse>();
        if (parsed is null)
        {
            return new GenerationResult<OpenAiQuestionsResponse>(GenerationErrorReasons.DeserializationError());
        }

        return new GenerationResult<OpenAiQuestionsResponse>(parsed);
    }
}