// <copyright file="LearningPlatformException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Results;

public record ExceptionResult(string code, string message);
public class LearningPlatformException : Exception
{
    public string Code { get; }

    public LearningPlatformException(ExceptionResult result)
        : base(result.message)
    {
        this.Code = result.code;
    }

    public LearningPlatformException(string code, string message)
        : base(message)
    {
        this.Code = code;
    }

    public LearningPlatformException(string code)
        : base(string.Empty)
    {
        this.Code = code;
    }
}