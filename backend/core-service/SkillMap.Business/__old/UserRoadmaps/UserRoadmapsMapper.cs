using Riok.Mapperly.Abstractions;

using SkillMap.Business.__old.UserRoadmaps.Models;
using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Business.UserRoadmaps;

[Mapper]
public static partial class UserRoadmapsMapper
{
    public static partial UserRoadmapDto ToUserRoadmapDto(this RoadmapBookmark userRoadmap);
}