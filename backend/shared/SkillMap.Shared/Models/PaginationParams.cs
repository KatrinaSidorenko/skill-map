// <copyright file="PaginationParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Models;

public record PaginationParams(int pageNumber, int pageSize)
{
    public int Skip => (this.pageNumber - 1) * this.pageSize;
}