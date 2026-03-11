using Newtonsoft.Json;

using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace SkillMap.Business.RoadmapBlueprints.GetRoadmapBlueprint;

public class RoadmapBlueprintDto
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("isSaved")]
    public bool IsSaved { get; set; }

    [JsonProperty("nodes")]
    public List<LearningItemDto> Nodes { get; set; }

    [JsonProperty("edges")]
    public List<LearningConnectionDto> Edges { get; set; }

    public static RoadmapBlueprintDto Create(RoadmapDto roadmapDto)
    {
        return new RoadmapBlueprintDto
        {
            Id = roadmapDto.Id,
            Title = roadmapDto.Title,
            Description = roadmapDto.Description,
            IsSaved = roadmapDto.IsSaved,
            Nodes = roadmapDto.Nodes.Select(LearningItemDto.Create).ToList(),
            Edges = roadmapDto.Edges.Select(LearningConnectionDto.Create).ToList()
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
