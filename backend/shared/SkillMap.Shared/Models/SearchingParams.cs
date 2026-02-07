// <copyright file="SearchingParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Models;

public record SearchingParams(string searchTermByName, PaginationParams paginationParams);

public static class DefaultParams
{
    public static readonly SearchingParams SearchingParams = new (string.Empty, new PaginationParams(1, 10));
}