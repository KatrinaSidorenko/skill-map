using Newtonsoft.Json;

using SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmap;

namespace SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmap;

public class GetPersonalRoadmapSummaryResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }

    public static GetPersonalRoadmapSummaryResponse Create(PersonalRoadmapSummaryDto blueprint)
    {
        return new GetPersonalRoadmapSummaryResponse
        {
            Id = blueprint.Id,
            Title = blueprint.Title,
            Description = blueprint.Description,
            ImageUrl = blueprint.ImageUrl
        };
    }
}
