using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Business.RoadmapBookmarks.Features.GetRoadmapBookmarks;
public record RoadmapBookmarksDto(IReadOnlyList<RoadmapBookmarkDto> Bookmarks)
{
    public static RoadmapBookmarksDto Create(IEnumerable<RoadmapBookmark> roadmapBookmarks)
    {
        var bookmarkDtos = roadmapBookmarks.Select(RoadmapBookmarkDto.Create).ToList();
        return new RoadmapBookmarksDto(bookmarkDtos);
    }
}
public record RoadmapBookmarkDto(long BookmarkId, string RoadmapId, bool IsActive, DateTime BookmarkedAt)
{
    public static RoadmapBookmarkDto Create(RoadmapBookmark roadmapBookmark)
    {
        return new RoadmapBookmarkDto(
            roadmapBookmark.Id,
            roadmapBookmark.RoadmapId,
            roadmapBookmark.IsActive,
            roadmapBookmark.CreatedAt
        );
    }
}
