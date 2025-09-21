using Riok.Mapperly.Abstractions;
using SkillMap.Api.UserRoadmaps.Models;

namespace SkillMap.Api.UserRoadmaps;

[Mapper]
public static partial class UserRoadmapsMapper
{
    public static partial UserRoadmapResponse ToUserRoadmapResponse(this Business.UserRoadmaps.Models.UserRoadmapDto userRoadmapDto);
}
