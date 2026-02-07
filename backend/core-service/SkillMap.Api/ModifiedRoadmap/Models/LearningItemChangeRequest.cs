using LearningPlatform.Roadmap.Business.Contracts.Models;

using Newtonsoft.Json;

namespace SkillMap.Api.ModifiedRoadmap.Models;

public class LearningItemsChangesRequest
{
    [JsonProperty("changes")]
    public List<LearningItemChangeRequest> Changes { get; set; }
}

public class LearningItemChangeRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }

    public NodeDto ToNodeDto()
    {
        return new NodeDto
        {
            Id = Id,
            Title = Title,
            Description = Description ?? string.Empty,
        };
    }
}