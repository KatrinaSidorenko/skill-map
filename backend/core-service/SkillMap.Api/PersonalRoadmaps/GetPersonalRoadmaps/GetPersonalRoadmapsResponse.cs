using LearningPlatform.Shared.Api.Searching;

using Newtonsoft.Json;

using SkillMap.Business.PersonalRoadmaps.Features.GetPersonalRoadmaps;
using SkillMap.Shared.Models;

namespace SkillMap.Api.PersonalRoadmaps.GetPersonalRoadmaps;

public class GetPersonalRoadmapsResponse
{
    public static PaginationResponse<GetPersonalRoadmapsResponseItem> CreatePaginationResult(PaginationResult<PersonalRoadmapDto> paginatedPersonalRoadmaps)
    {
        var responseItems = paginatedPersonalRoadmaps.Result.Select(r => GetPersonalRoadmapsResponseItem.Create(r)).ToList();
        return new PaginationResponse<GetPersonalRoadmapsResponseItem>
        {
            Total = paginatedPersonalRoadmaps.TotalCount,
            Items = responseItems
        };
    }
}

public class GetPersonalRoadmapsResponseItem
{
    [JsonProperty("roadmapId")]
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