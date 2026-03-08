using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Core.PersonalizedRoadmaps;

public class RoadmapWorkspaceSnapshot : TrackedEntity
{
    public long RoadmapForkId { get; private set; }
    public byte[]? Content { get; private set; } // gzipped JSON
    public int LatestVersion { get; private set; }

    public virtual RoadmapWorkspace RoadmapFork { get; set; }
    public RoadmapWorkspaceSnapshot() { }
    public RoadmapWorkspaceSnapshot(long userRoadmapId, byte[]? content, int latestVersion)
    {
        RoadmapForkId = userRoadmapId;
        Content = content;
        LatestVersion = latestVersion;
    }
}