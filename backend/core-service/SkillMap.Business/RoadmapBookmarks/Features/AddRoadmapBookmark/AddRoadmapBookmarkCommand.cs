namespace SkillMap.Business.RoadmapBookmarks.Features.AddRoadmapBookmark;
public record AddRoadmapBookmarkCommand(long UserId, string RoadmapId) : ICommand<long>;
