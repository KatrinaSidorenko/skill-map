// <copyright file="PaginationParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Models;

public record PaginationParams(int PageNumber, int PageSize)
{
    public int Skip => (this.PageNumber - 1) * this.PageSize;
}