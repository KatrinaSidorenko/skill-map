using System.Text.Json.Serialization;

using SkillMap.Business.RoadmapsWorkspace.Features.GetLearningItemMaterials;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Api.RoadmapsWorkspace.GetLearningItemMaterials;

public class GetLearningItemMaterialsResponse
{
    [JsonPropertyName("materials")]
    public List<LearningItemMaterialResponse> Materials { get; set; }

    public static GetLearningItemMaterialsResponse Create(List<LearningItemMaterialDto> dtos)
        => new() { Materials = dtos.Select(LearningItemMaterialResponse.Create).ToList() };
}

public class LearningItemMaterialResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    public static LearningItemMaterialResponse Create(LearningItemMaterialDto dto)
        => new()
        {
            Id = dto.Id,
            Title = dto.Title,
            Url = dto.Url,
            Type = dto.Type.ToTypeString()
        };
}
