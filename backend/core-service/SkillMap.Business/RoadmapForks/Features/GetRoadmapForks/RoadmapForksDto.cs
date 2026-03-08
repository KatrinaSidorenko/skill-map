using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Business.RoadmapBookmarks.Features.GetRoadmapBookmarks;
public record RoadmapForksDto(IReadOnlyList<RoadmapBookmarkDto> Bookmarks)
{
    public static RoadmapForksDto Create(IEnumerable<RoadmapFork> roadmapBookmarks)
    {
        var bookmarkDtos = roadmapBookmarks.Select(RoadmapBookmarkDto.Create).ToList();
        return new RoadmapForksDto(bookmarkDtos);
    }
}
public record RoadmapBookmarkDto(long BookmarkId, string RoadmapId, bool IsActive, DateTime BookmarkedAt)
{
    public static RoadmapBookmarkDto Create(RoadmapFork roadmapBookmark)
    {
        return new RoadmapBookmarkDto(
            roadmapBookmark.Id,
            roadmapBookmark.RoadmapId,
            roadmapBookmark.IsActive,
            roadmapBookmark.CreatedAt
        );
    }
}
