using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Core.PersonalizedRoadmaps;

public class RoadmapWorkspaceSnapshot : TrackedEntity
{
    public long BookmarkId { get; set; }
    public byte[]? Content { get; set; } // gzipped JSON
    public int LatestVersion { get; set; }

    public virtual RoadmapBookmark UserRoadmap { get; set; }
    public RoadmapWorkspaceSnapshot() { }
    public RoadmapWorkspaceSnapshot(long userRoadmapId, byte[]? content, int latestVersion)
    {
        BookmarkId = userRoadmapId;
        Content = content;
        LatestVersion = latestVersion;
    }
}