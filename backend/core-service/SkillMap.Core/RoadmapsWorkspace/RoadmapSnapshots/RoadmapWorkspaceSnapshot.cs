using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

public class RoadmapSnapshotMetadata
{
    [JsonProperty("progress")]
    public double Progress { get; private set; }
    [JsonProperty("status")]
    public LearningStatus Status { get; private set; }

    public RoadmapSnapshotMetadata(double progress, LearningStatus status)
    {
        Progress = progress;
        Status = status;
    }
}

public class RoadmapWorkspaceSnapshot : TrackedEntity
{
    public long RoadmapWorkspaceId { get; private set; }
    public byte[]? Content { get; private set; } // gzipped JSON
    public string? Metadata { get; private set; }
    public int Version { get; private set; }

    public virtual RoadmapWorkspace RoadmapWorkspace { get; set; }
    public RoadmapWorkspaceSnapshot() { }

    public RoadmapWorkspaceSnapshot(long userRoadmapId, byte[]? content, int latestVersion)
    {
        RoadmapWorkspaceId = userRoadmapId;
        Content = content ?? [];
        Version = latestVersion;
        Metadata = null;
    }

    public RoadmapWorkspaceSnapshot(long workspaceId) : this(workspaceId, null, 0) { }

    public void SetContent(byte[] content)
    {
        Content = content;
    }

    public void SetVersion(int version)
    {
        Version = version;
    }

    // todo: logic of serialization and desiralization seems like to be logical here
    public void SetMetadata(RoadmapSnapshotMetadata metadata)
    {
        Metadata = JsonConvert.SerializeObject(metadata);
    }
}