// <copyright file="Result.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;

namespace SkillMap.Shared.Results;

public record ExceptionResponse(HttpStatusCode statusCode, string message);
public class Result<T>
{
    public bool IsSuccessful { get; }

    public bool IsFailed => !this.IsSuccessful;

    public string Code { get; }

    public string Message { get; }

    public T Data { get; }

    public bool HasData => this.Data != null;

    public Result(bool isSuccessful, T data)
    {
        this.IsSuccessful = isSuccessful;
        this.Data = data;
    }

    public Result(bool isSuccessful, string message, string code)
    {
        this.IsSuccessful = isSuccessful;
        this.Message = message;
        this.Code = code;
    }

    public Result(bool isSuccessful)
    {
        this.IsSuccessful = isSuccessful;
    }
}

public static class Result
{
    public static Result<T> Success<T>(T data) => new(true, data);

    public static Result<T> Success<T>() => new(true);

    public static Result<T> Failure<T>(string code, string message) => new(false, message, code);
}