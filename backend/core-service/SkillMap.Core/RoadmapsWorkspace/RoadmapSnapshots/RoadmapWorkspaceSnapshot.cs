using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

public class RoadmapSnapshotMetadata
{
    [JsonProperty("title")]
    public string Title { get; private set; }
    [JsonProperty("description")]
    public string Description { get; private set; }
    [JsonProperty("imageUrl")]
    public string ImageUrl { get; private set; }
    [JsonProperty("progress")]
    public double Progress { get; private set; }
    [JsonProperty("status")]
    public LearningStatus Status { get; private set; }

    public RoadmapSnapshotMetadata(double progress, LearningStatus status, string title, string description, string imageUrl)
    {
        Progress = progress;
        Status = status;
        Title = title;
        Description = description;
        ImageUrl = imageUrl;
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
    public void SetMetadata(string metadata)
    {
        Metadata = metadata;
    }
}