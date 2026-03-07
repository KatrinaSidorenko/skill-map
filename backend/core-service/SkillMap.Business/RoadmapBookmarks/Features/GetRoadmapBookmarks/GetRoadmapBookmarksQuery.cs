namespace SkillMap.Business.RoadmapBookmarks.Features.GetRoadmapBookmarks;
public record GetRoadmapBookmarksQuery(long UserId, bool IsActive) : ICommand<RoadmapBookmarksDto>;
