// <copyright file="ValidationExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation.Results;

namespace SkillMap.Shared.Extensions;

public static class ValidationExtensions
{
    public static List<string> GetErrors(this ValidationResult result)
        => result.Errors.Select(e => e.PropertyName + ". Message: " + e.ErrorMessage + "Code:" + e.ErrorCode).ToList();
}