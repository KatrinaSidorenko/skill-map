using System.Text.Json.Serialization;

using Newtonsoft.Json;

using SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprint;

namespace SkillMap.Api.RoadmapBlueprints.GetRoadmapBlueprint;

public class GetRoadmapBlueprintResponse
{
    [JsonProperty("roadmap")]
    public RoadmapResponse Roadmap { get; set; }

    public static GetRoadmapBlueprintResponse Create(RoadmapBlueprintDto roadmapBlueprint)
    {
        return new GetRoadmapBlueprintResponse
        {
            Roadmap = RoadmapResponse.Create(roadmapBlueprint)
        };
    }
}

public class RoadmapResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonPropertyName("workspaceId")]
    public string WorkspaceId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("sourceLink")]
    public string SourceLink { get; set; }

    [JsonProperty("isSaved")]
    public bool IsSaved { get; set; }

    [JsonProperty("items")]
    public List<RoadmapNode> Items { get; set; }

    [JsonProperty("connections")]
    public List<RoadmapEdge> Connections { get; set; }

    public static RoadmapResponse Create(RoadmapBlueprintDto dto)
    {
        return new RoadmapResponse
        {
            Id = dto.Id,
            WorkspaceId = dto.WorkspaceId,
            Title = dto.Title,
            Description = dto.Description,
            IsSaved = dto.IsSaved,
            SourceLink = "https://roadmap.sh/", // todo: remove hardcode
            Items = dto.Items.Select(RoadmapNode.Create).ToList(),
            Connections = dto.Connections.Select(RoadmapEdge.Create).ToList()
        };
    }
}

public class RoadmapNode
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    public static RoadmapNode Create(LearningItemDto item)
    {
        return new RoadmapNode
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description
        };
    }
}

public class RoadmapEdge
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("source")]
    public string? Source { get; set; }

    [JsonProperty("target")]
    public string? Target { get; set; }

    public static RoadmapEdge Create(LearningConnectionDto connection)
    {
        return new RoadmapEdge
        {
            Id = connection.Id,
            Source = connection.Source,
            Target = connection.Target
        };
    }
}