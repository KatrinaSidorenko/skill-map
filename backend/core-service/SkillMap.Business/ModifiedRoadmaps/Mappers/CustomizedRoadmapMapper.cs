using LearningPlatform.Roadmap.Business.Contracts.Models;

using Riok.Mapperly.Abstractions;

using SkillMap.Business.ModifiedRoadmaps.Models;
using SkillMap.Business.Roadmaps.Models;

namespace SkillMap.Business.ModifiedRoadmaps.Mappers;

[Mapper]
public static partial class CustomizedRoadmapMapper
{
    [MapProperty(nameof(PlainRoadmapDto.CreatedAt), nameof(PlainRoadmapWithDetailsDto.SavedAt))]
    public static partial PlainRoadmapWithDetailsDto ToPlainRoadmapWithDetailsDto(this PlainRoadmapDto roadmap);
}