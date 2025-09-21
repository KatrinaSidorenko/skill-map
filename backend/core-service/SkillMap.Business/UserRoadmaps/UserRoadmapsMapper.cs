using Riok.Mapperly.Abstractions;
using SkillMap.Business.UserRoadmaps.Models;

namespace SkillMap.Business.UserRoadmaps;

[Mapper]
public static partial class UserRoadmapsMapper
{
    public static partial UserRoadmapDto ToUserRoadmapDto(this Core.Entities.UserRoadmap userRoadmap);
}
