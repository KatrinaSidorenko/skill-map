using Riok.Mapperly.Abstractions;
using SkillMap.Api.ModifiedRoadmap.Models;
using SkillMap.Business.ModifiedRoadmaps.Models;

namespace SkillMap.Api.ModifiedRoadmap;

[Mapper]
public static partial class ModifiedRoadmapsMapper
{
    public static partial SavedPlainRoadmapResponse ToResponse(this PlainRoadmapWithDetailsDto dto);
}
