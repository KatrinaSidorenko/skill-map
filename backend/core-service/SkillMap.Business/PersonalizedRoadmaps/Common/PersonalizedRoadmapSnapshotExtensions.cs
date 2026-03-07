using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Shared.Gzip;

namespace SkillMap.Business.PersonalizedRoadmaps.Common;
internal static class PersonalizedRoadmapSnapshotExtensions
{
    public static async Task<RoadmapSnapshot> GetPersonalizedRoadmapSnapshot(this PersonalizedRoadmapSnapshot snapshot, CancellationToken ct)
        => await snapshot.Content?.InGzipJsonObjectUtf8<RoadmapSnapshot>(ct);
}