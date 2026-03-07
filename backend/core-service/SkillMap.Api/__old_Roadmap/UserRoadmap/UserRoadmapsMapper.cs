using Riok.Mapperly.Abstractions;

using SkillMap.Api.__old_Roadmap.UserRoadmap.Models;
using SkillMap.Business.__old.UserRoadmaps.Models;

namespace SkillMap.Api.UserRoadmaps;

[Mapper]
public static partial class UserRoadmapsMapper
{
    public static partial UserRoadmapResponse ToUserRoadmapResponse(this UserRoadmapDto userRoadmapDto);
}