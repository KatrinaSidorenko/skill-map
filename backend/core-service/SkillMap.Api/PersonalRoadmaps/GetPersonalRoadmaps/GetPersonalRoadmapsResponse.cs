using Newtonsoft.Json;

using SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;
using SkillMap.Shared.Models;

namespace SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmaps;

public class GetPersonalRoadmapsResponse
{
    public static PaginationResult<List<GetPersonalRoadmapsResponseItem>> CreatePaginationResult(PaginationResult<PersonalRoadmapsDto> paginatedPersonalRoadmaps)
    {
        var responseItems = paginatedPersonalRoadmaps.Result.PersonalRoadmaps.Select(GetPersonalRoadmapsResponseItem.Create).ToList();
        return new PaginationResult<List<GetPersonalRoadmapsResponseItem>>
        {
            TotalCount = paginatedPersonalRoadmaps.TotalCount,
            Result = responseItems
        };
    }
}

public class GetPersonalRoadmapsResponseItem
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    public static GetPersonalRoadmapsResponseItem Create(PersonalRoadmapDto personalRoadmap)
    {
        return new GetPersonalRoadmapsResponseItem
        {
            Id = personalRoadmap.Id,
            Title = personalRoadmap.Title,
            Description = personalRoadmap.Description,
            ImageUrl = personalRoadmap.ImageUrl,
            CreatedAt = personalRoadmap.CreatedAt
        };
    }
}
