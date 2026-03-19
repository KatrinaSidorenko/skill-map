// <copyright file="PaginationParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace SkillMap.Shared.Models;

public class PaginationParams
{
    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; } = 1;
    [JsonProperty("pageSize")]
    public int PageSize { get; set; } = 10;
    public int Skip => (this.PageNumber - 1) * this.PageSize;

    public PaginationParams() { }
    public PaginationParams(int pageNum, int pageSize)
    {
        PageNumber = pageNum < 1 ? 1 : pageNum;
        PageSize = pageSize < 1 ? 10 : pageSize;
    }
}