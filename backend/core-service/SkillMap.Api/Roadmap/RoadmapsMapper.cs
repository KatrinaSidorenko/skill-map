using LearningPlatform.Roadmap.Business.Contracts.Models;
using Riok.Mapperly.Abstractions;
using SkillMap.Api.Roadmaps.Models;

namespace SkillMap.Api.Roadmaps;

[Mapper]
public static partial class RoadmapsMapper
{
    public static partial PlainRoadmapResponse ToRoadmapResponse(this PlainRoadmapDto roadmapDto);
}
