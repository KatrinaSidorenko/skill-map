using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using Newtonsoft.Json;

namespace SkillMap.Api.Roadmap.Models;

public class CreatePlainRoadmapRequest
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
    [JsonProperty("isPublic")]
    public bool IsPublic { get; set; }
}

public class CreateNodeRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }

    public NodeDto NodeDto => new NodeDto
    {
        Id = Id,
        Title = Title,
        Description = Description,
        Type = NodeType.Topic, // todo: how to differentiate this?
        AdditionalProps = new Dictionary<string, string>()
    };
}

public class CreateEdgeRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("source")]
    public string Source { get; set; }
    [JsonProperty("target")]
    public string Target { get; set; }

    public EdgeDto EdgeDto => new EdgeDto
    {
        Id = Id,
        Source = new NodeDto { ExternalId = Source },
        Target = new NodeDto { ExternalId = Target },
    };
}
