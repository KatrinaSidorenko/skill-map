using Newtonsoft.Json;

using SkillMap.Business.RoadmapsWorkspace.Features.GetRoadmapWorkspaces;
using SkillMap.Core.Constants;

namespace SkillMap.Api.RoadmapsWorkspace.GetRoadmapWorkspaceSummary;

public class GetRoadmapWorkspaceSummaryResponse
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

    public static GetRoadmapWorkspaceSummaryResponse Create(RoadmapWorkspaceSummaryDto dto)
    {
        return new GetRoadmapWorkspaceSummaryResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Progress = dto.Progress,
            SavedAt = dto.SavedAt,
            Status = dto.Status.ToStatusString()
        };
    }
}
