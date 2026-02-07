// <copyright file="DictionaryExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default = default)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return @default;
    }

    public static TValue? GetOrDefaultAsNullable<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : struct
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }
}