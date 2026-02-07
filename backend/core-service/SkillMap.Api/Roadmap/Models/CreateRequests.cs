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

public class UpdatePlainRoadmapRequest
{
    [JsonProperty("title")]
    public string? Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }
    [JsonProperty("isPublic")]
    public bool? IsPublic { get; set; }

    public NodeDto ToRoadmapNodeDto(string roadmapId, string ownerId)
    {
        return new NodeDto
        {
            Id = roadmapId,
            Title = Title,
            Description = Description,
            Type = NodeType.Roadmap,
            AdditionalProps = new Dictionary<string, string>
            {
                { NodeProps.ImageUrl, ImageUrl },
                { NodeProps.IsPublic, IsPublic?.ToString().ToLower() },
                { NodeProps.OwnerId, ownerId }
            }
        };
    }
}

public class CreateNodeRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }

    public NodeDto ToNodeDto(string roadmapId)
    {
        return new NodeDto
        {
            Id = Id,
            Title = Title,
            Description = Description,
            Type = NodeType.Topic, // todo: how to differentiate this?
            AdditionalProps = new Dictionary<string, string>
            {
                { NodeProps.RoadmapId, roadmapId }
            }
        };
    }
}

public class CreateEdgeRequest
{
    [JsonProperty("sourceId")]
    public string SourceId { get; set; }
    [JsonProperty("targetId")]
    public string TargetId { get; set; }

    [JsonIgnore]
    public EdgeDto EdgeDto => new EdgeDto
    {
        Source = new NodeDto { ExternalId = SourceId },
        Target = new NodeDto { ExternalId = TargetId },
    };
}