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
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
 [JsonProperty("description")]
    public string Description { get; set; }
    
    [JsonProperty("isSaved")]
    public bool IsSaved { get; set; }
    
    [JsonProperty("nodes")]
    public List<RoadmapNode> Nodes { get; set; }
    
    [JsonProperty("edges")]
    public List<RoadmapEdge> Edges { get; set; }

    public static RoadmapResponse Create(RoadmapBlueprintDto dto)
  {
        return new RoadmapResponse
        {
            Id = dto.Id,
            Title = dto.Title,
        Description = dto.Description,
            IsSaved = dto.IsSaved,
  Nodes = dto.Nodes.Select(RoadmapNode.Create).ToList(),
            Edges = dto.Edges.Select(RoadmapEdge.Create).ToList()
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
