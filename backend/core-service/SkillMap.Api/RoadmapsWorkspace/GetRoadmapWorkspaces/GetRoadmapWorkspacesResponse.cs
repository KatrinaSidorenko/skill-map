using LearningPlatform.Shared.Api.Searching;

using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;
using SkillMap.Core.Constants;
using SkillMap.Shared.Models;

namespace SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaces;

public class GetRoadmapWorkspacesResponse
{
    public static PaginationResponse<GetRoadmapWorkspacesResponseItem> CreatePaginationResult(PaginationResult<RoadmapWorkspaceSummaryDto> paginatedWorkspaces)
    {
        var responseItems = paginatedWorkspaces.Result.Select(r => GetRoadmapWorkspacesResponseItem.Create(r)).ToList();
        return new PaginationResponse<GetRoadmapWorkspacesResponseItem>
        {
            Total = paginatedWorkspaces.TotalCount,
            Items = responseItems
        };
    }
}

public class GetRoadmapWorkspacesResponseItem
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }

    [JsonProperty("progress")]
    public double Progress { get; set; }

    [JsonProperty("savedAt")]
    public DateTime SavedAt { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    public static GetRoadmapWorkspacesResponseItem Create(RoadmapWorkspaceSummaryDto workspace)
    {
        return new GetRoadmapWorkspacesResponseItem
        {
            Id = workspace.Id,
            Title = workspace.Title,
            Description = workspace.Description,
            ImageUrl = workspace.ImageUrl,
            Progress = workspace.Progress,
            SavedAt = workspace.SavedAt,
            Status = workspace.Status.ToStatusString()
        };
    }
}