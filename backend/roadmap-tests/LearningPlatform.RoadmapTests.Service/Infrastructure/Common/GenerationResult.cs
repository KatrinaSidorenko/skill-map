using SkillMap.Shared.Results;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.Common;

public record ErrorReason(string Code, string Description)
{
    public override string ToString() => $"{Code}: {Description}";
}

public static class GenerationErrorReasons
{
    public const string INSUFFICIENT_DATA = "INSUFFICIENT_DATA";
    public const string GENERATION_TIMEOUT = "GENERATION_TIMEOUT";
    public const string EXTERNAL_SERVICE_FAILURE = "EXTERNAL_SERVICE_FAILURE";
    public const string OPEN_AI_REFUSAL = "OPEN_AI_REFUSAL";
    public const string FILTERED_BY_OPEN_AI = "FILTERED_BY_OPEN_AI";
    public const string OUTPUT_TOKEN_LIMIT_EXCEEDED = "OUTPUT_TOKEN_LIMIT_EXCEEDED";
    public const string OPEN_AI_OUPUT_IS_EMPTY = "OPEN_AI_OUPUT_IS_EMPTY";
    public const string DESERIALIZATION_ERROR = "DESERIALIZATION_ERROR";
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    public const string NOT_FOUND = "NOT_FOUND";

    public static readonly ErrorReason InsufficientData = new(
        INSUFFICIENT_DATA,
        "Not enough data to generate questions for the given topic.");
    public static readonly ErrorReason GenerationTimeout = new(
        GENERATION_TIMEOUT,
        "The question generation process timed out.");
    public static readonly ErrorReason ExternalServiceFailure = new(
        EXTERNAL_SERVICE_FAILURE,
        "An external service required for question generation failed.");
    public static readonly ErrorReason OpenAIRefusal = new(
        OPEN_AI_REFUSAL,
        "The OpenAI service refused to process the request.");
    public static readonly ErrorReason FilteredByOpenAI = new(
        FILTERED_BY_OPEN_AI,
        "The generated content was filtered out by OpenAI's content policies.");
    public static readonly ErrorReason OutputTokenLimitExceeded = new(
        OUTPUT_TOKEN_LIMIT_EXCEEDED,
        "The generated output exceeded the token limit.");
    public static ErrorReason OpenAIOutputIsEmpty(string debugInfo = null) => new(
        OPEN_AI_OUPUT_IS_EMPTY,
        debugInfo);

    public static ErrorReason DeserializationError(string debugInfo = null) => new(
        DESERIALIZATION_ERROR,
        debugInfo ?? "Failed to deserialize the generated output.");
    public static ErrorReason InternalError(string debugInfo = null) => new(
        INTERNAL_ERROR,
        debugInfo ?? "An internal error occurred during question generation.");
    public static ErrorReason NotFound(string debugInfo = null) => new(
        NOT_FOUND,
        debugInfo ?? "No relevant data found for question generation.");
}
public class GenerationResult<T> : Result<T>
{
    public ErrorReason Reason { get; set; }
    public GenerationResult(bool isSuccessful, T data) : base(isSuccessful, data) { }
    public GenerationResult(T data) : base(true, data) { }
    public GenerationResult(ErrorReason errorReason) : base(false, default!)
    {
        Reason = errorReason;
    }
}

public static class GenerationResultExtensions
{
    public static GenerationResult<TTo> ToGenerationResult<TTo, TFrom>(
        this GenerationResult<TFrom> source,
        Func<TFrom, TTo> mapData = null)
    {
        if (source.IsSuccessful)
        {
            var mappedData = mapData(source.Data!);
            return new GenerationResult<TTo>(true, mappedData);
        }
        else
        {
            return new GenerationResult<TTo>(source.Reason!);
        }
    }
}