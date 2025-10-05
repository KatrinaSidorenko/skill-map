using Riok.Mapperly.Abstractions;
using SkillMap.Api.ModifiedRoadmap.Models;
using SkillMap.Business.ModifiedRoadmaps.Models;
using SkillMap.Business.Roadmaps.Models;

namespace SkillMap.Api.ModifiedRoadmap;

[Mapper]
public static partial class ModifiedRoadmapsMapper
{
    public static partial SavedPlainRoadmapResponse ToResponse(this PlainRoadmapWithDetailsDto dto);
    public static partial LearningItemChange ToChnage(this LearningItemChangeRequest request);
}
