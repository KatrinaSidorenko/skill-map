// <copyright file="NumericExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Extensions;

public static class NumericExtensions
{
    public static long? GetLongOrDefault(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (long.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }

    public static string ToStringWithoutHyphens(this Guid guid) => guid.ToString("N");

    public static long EnsureParseLong(this string value)
    {
        if (!long.TryParse(value, out var result))
        {
            throw new FormatException($"The value '{value}' is not a valid long integer.");
        }

        return result;
    }
}