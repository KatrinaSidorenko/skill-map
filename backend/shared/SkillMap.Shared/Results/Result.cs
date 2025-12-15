using System.Net;

namespace SkillMap.Shared.Results;

public record ExceptionResponse(HttpStatusCode StatusCode, string Message);
public class Result<T>
{
    public bool IsSuccessful { get; }
    public bool IsFailed => !IsSuccessful;
    public string Code { get; }
    public string Message { get; }
    public T Data { get; }
    public bool HasData => Data != null;

    public Result(bool isSuccessful, T data)
    {
        IsSuccessful = isSuccessful;
        Data = data;
    }

    public Result(bool isSuccessful, string message, string code)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        Code = code;
    }

    public Result(bool isSuccessful)
    {
        IsSuccessful = isSuccessful;
    }

}

public static class Result
{
    public static Result<T> Success<T>(T data) => new(true, data);
    public static Result<T> Success<T>() => new(true);
    public static Result<T> Failure<T>(string code, string message) => new(false, message, code);
}
