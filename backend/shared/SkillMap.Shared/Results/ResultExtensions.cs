using System.Net;

namespace SkillMap.Shared.Results;

public record ResponseInfo(string Code, string Message);

public static class ResultExtension
{
    //public static Result<T> GetFailureResult<T>(this ResultData resultData, string additionalMessage = "")
    //    => new(false, resultData.Message.Append(additionalMessage), resultData.Code);

    public static ResponseInfo GetResultResponse<T>(this Result<T> result)
        => new(result.Code, result.Message);
    public static ExceptionResult ToExceptionResult<T>(this Result<T> result)
        => new(result.Code, result.Message);

    public static bool IsBadRequest<T>(this Result<T> result)
        => !result.IsSuccessful && result.Code.Contains(ErrorCode.USER_INPUT_ERROR_PREFIX);
    public static bool IsInternalError<T>(this Result<T> result)
        => !result.IsSuccessful && result.Code.Contains(ErrorCode.SYSTEM_ERROR_PREFIX);

    public static HttpStatusCode ToStatusCode(this LearningPlatformException ex) 
        => ex?.Code?.ToStatusCode() ?? HttpStatusCode.InternalServerError;

    private static HttpStatusCode ToStatusCode(this string code)
        => code switch
        {
            var c when c.Contains(ErrorCode.USER_INPUT_ERROR_PREFIX) => HttpStatusCode.BadRequest,
            var c when c.Contains(ErrorCode.SYSTEM_ERROR_PREFIX) => HttpStatusCode.InternalServerError,
            var c when c.Contains(ErrorCode.NOT_FOUND) => HttpStatusCode.NotFound,
            var c when c.Contains(ErrorCode.VALIDATION_ERROR) => HttpStatusCode.BadRequest,
            // var c when c.Contains(ErrorCode.UNAUTHORIZED_PREFIX) => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };
}
