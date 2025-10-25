using LearningPlatform.Roadmap.Business.Contracts.Models;
using Riok.Mapperly.Abstractions;
using SkillMap.Api.Roadmap.Models;
using SkillMap.Api.Roadmaps.Models;

namespace SkillMap.Api.Roadmaps;

[Mapper]
public static partial class RoadmapsMapper
{
    public static partial PlainRoadmapResponse ToRoadmapResponse(this PlainRoadmapDto roadmapDto);

    [MapProperty(nameof(ResourceDto.Link), nameof(LearningItemMaterialResponse.Url))]
    public static partial LearningItemMaterialResponse ToMaterialResponse(this ResourceDto materialDto);
    public static partial PlainRoadmapDto ToPlainRoadmapDto(this CreatePlainRoadmapRequest roadmapRequest);
}
