using LearningPlatform.Shared.Api.Searching;

using Newtonsoft.Json;

using SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprintSummary;
using SkillMap.Shared.Models;

namespace SkillMap.Api.RoadmapBlueprints.GetRoadmapBlueprintsSummary;

public class GetRoadmapBlueprintsSummaryResponse
{
    public static PaginationResponse<GetRoadmapBlueprintsSummaryResponseItem> CreatePaginationResult(PaginationResult<RoadmapBlueprintSummaryDto> paginatedBlueprints)
    {
        var responseItems = paginatedBlueprints.Result.Select(GetRoadmapBlueprintsSummaryResponseItem.Create).ToList();
        return new PaginationResponse<GetRoadmapBlueprintsSummaryResponseItem>
        {
            Total = paginatedBlueprints.TotalCount,
            Items = responseItems
        };
    }
}

public class GetRoadmapBlueprintsSummaryResponseItem
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
    [JsonProperty("isSaved")]
    public bool IsSaved { get; set; }

    public static GetRoadmapBlueprintsSummaryResponseItem Create(RoadmapBlueprintSummaryDto blueprint)
    {
        return new GetRoadmapBlueprintsSummaryResponseItem
        {
            Id = blueprint.Id,
            Title = blueprint.Title,
            Description = blueprint.Description,
            ImageUrl = blueprint.ImageUrl,
            IsSaved = blueprint.IsSaved
        };
    }
}