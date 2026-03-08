namespace SkillMap.Business.RoadmapBookmarks.Features.GetRoadmapBookmarks;
public record GetRoadmapForksQuery(long UserId, bool IsActive) : ICommand<RoadmapForksDto>;
