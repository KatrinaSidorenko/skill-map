using Newtonsoft.Json;

using SkillMap.Business.PersonalRoadmaps.Features.UpdatePersonalRoadmap;

namespace SkillMap.Api.PersonalRoadmaps.UpdatePersonalRoadmap;

public class UpdatePersonalRoadmapRequest
{
    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }

    public UpdatePersonalRoadmapCommand ToCommand(long personalRoadmapId, long userId)
        => new(personalRoadmapId, userId, Title, Description, ImageUrl);
}
