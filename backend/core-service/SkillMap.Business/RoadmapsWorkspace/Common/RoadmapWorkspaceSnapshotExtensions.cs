using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Gzip;

namespace SkillMap.Business.PersonalizedRoadmaps.Common;
internal static class RoadmapWorkspaceSnapshotExtensions
{
    public static async Task<RoadmapSnapshot> GetRoadmapSnapshot(this RoadmapWorkspaceSnapshot snapshot, CancellationToken ct)
        => await snapshot.Content?.InGzipJsonObjectUtf8<RoadmapSnapshot>(ct);
    public static async Task SetRoadmapSnapshot(this RoadmapWorkspaceSnapshot snapshot, RoadmapSnapshot roadmapSnapshot, CancellationToken ct)
        => snapshot.SetContent(await roadmapSnapshot.GzipJsonObjectUtf8(ct));

    public static RoadmapSnapshotMetadata ParseMetadata(this RoadmapWorkspaceSnapshot snapshot)
        => snapshot?.Metadata?.JsonDeserializeOrDefault<RoadmapSnapshotMetadata>();
    public static void SetMetadata(this RoadmapWorkspaceSnapshot snapshot, RoadmapSnapshotMetadata metadata)
        => snapshot?.SetMetadata(metadata.JsonSerializeOrDefault());
}