namespace SkillMap.Shared.Results;

public record ResultData(string Code, string Message);

public static class ResultExtension
{
    //public static Result<T> GetFailureResult<T>(this ResultData resultData, string additionalMessage = "")
    //    => new(false, resultData.Message.Append(additionalMessage), resultData.Code);

    public static ResultData GetResultResponse<T>(this Result<T> result)
        => new(result.Code, result.Message);

    public static bool IsBadRequest<T>(this Result<T> result)
        => !result.IsSuccessful && result.Code.Contains(ErrorCode.USER_INPUT_ERROR_PREFIX);
    public static bool IsInternalError<T>(this Result<T> result)
        => !result.IsSuccessful && result.Code.Contains(ErrorCode.SYSTEM_ERROR_PREFIX);
}
