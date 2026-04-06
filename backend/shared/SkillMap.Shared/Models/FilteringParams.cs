// <copyright file="SearchingParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Models;

public record FilteringParams(string SearchTerm, PaginationParams PaginationParams);

public static class DefaultParams
{
    public static readonly FilteringParams SearchingParams = new(string.Empty, new PaginationParams { PageNumber = 1, PageSize = 10 });
}