using Newtonsoft.Json;

using SkillMap.Business.PersonalRoadmaps.Features.CreatePersonalRoadmap;

namespace SkillMap.Api.PersonalRoadmaps.CreatePersonalRoadmap;

public class CreatePersonalRoadmapRequest
{
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }

    public CreatePersonalRoadmapCommand ToCommand(long userId)
        => new(userId, Title, Description, ImageUrl);
}