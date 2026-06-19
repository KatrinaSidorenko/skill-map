using LearningPlatform.Roadmap.Business.Contracts.Models;

using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprint;

public class RoadmapBlueprintDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("workspaceId")]
    public string WorkspaceId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("isSaved")]
    public bool IsSaved { get; set; }
    [JsonProperty("sourceLink")]
    public string? SourceLink { get; set; }

    [JsonProperty("items")]
    public List<LearningItemDto> Items { get; set; }

    [JsonProperty("connections")]
    public List<LearningConnectionDto> Connections { get; set; }

    public static RoadmapBlueprintDto Create(RoadmapDto roadmapDto, bool isSaved, string workspaceId)
    {
        return new RoadmapBlueprintDto
        {
            Id = roadmapDto.Id,
            WorkspaceId = workspaceId,
            Title = roadmapDto.Title,
            Description = roadmapDto.Description,
            IsSaved = isSaved,
            Items = roadmapDto.Nodes.Select(LearningItemDto.Create).ToList(),
            Connections = roadmapDto.Edges.Select(LearningConnectionDto.Create).ToList(),
            SourceLink = roadmapDto.SourceLink
        };
    }
}

public class LearningItemDto
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    public static LearningItemDto Create(Node node)
    {
        return new LearningItemDto
        {
            Id = node.Id,
            Title = node.Title,
            Description = node.Description
        };
    }
}

public class LearningConnectionDto
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("source")]
    public string? Source { get; set; }

    [JsonProperty("target")]
    public string? Target { get; set; }

    public static LearningConnectionDto Create(Edge edge)
    {
        return new LearningConnectionDto
        {
            Id = edge.Id,
            Source = edge.Source,
            Target = edge.Target
        };
    }
}