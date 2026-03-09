using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Core.PersonalizedRoadmaps;

public class RoadmapWorkspaceSnapshot : TrackedEntity
{
    public long RoadmapWorkspaceId { get; private set; }
    public byte[]? Content { get; private set; } // gzipped JSON
    public int Version { get; private set; }

    public virtual RoadmapWorkspace RoadmapWorkspace { get; set; }
    public RoadmapWorkspaceSnapshot() { }

    public RoadmapWorkspaceSnapshot(long userRoadmapId, byte[]? content, int latestVersion)
    {
        RoadmapWorkspaceId = userRoadmapId;
        Content = content ?? Array.Empty<byte>();
        Version = latestVersion;
    }

    public RoadmapWorkspaceSnapshot(long workspaceId) : this(workspaceId, null, 0) { }

    public void SetContent(byte[] content)
    {
        Content = content;
    }

    public void IncrementVersion(int latestVersion)
    {
        Version = ++latestVersion;
    }
}