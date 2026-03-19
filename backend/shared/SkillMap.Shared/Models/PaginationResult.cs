// <copyright file="PaginationResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SkillMap.Shared.Models;

public class PaginationResult<T>
{
    public List<T> Result { get; set; }
    public int TotalCount { get; set; }
}