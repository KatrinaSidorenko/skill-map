using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Gzip;

namespace SkillMap.Business.PersonalizedRoadmaps.Common;
internal static class RoadmapWorkspaceSnapshotExtensions
{
    public static async Task<RoadmapSnapshot> GetRoadmapSnapshot(this RoadmapWorkspaceSnapshot snapshot, CancellationToken ct)
        => (await snapshot.Content?.InGzipJsonObjectUtf8<RoadmapSnapshot>(ct)).EmptyOnNull(snapshot.RoadmapWorkspace.ActualRoadmapId);
    public static async Task SetRoadmapSnapshot(this RoadmapWorkspaceSnapshot snapshot, RoadmapSnapshot roadmapSnapshot, CancellationToken ct)
        => snapshot.SetContent(await roadmapSnapshot.GzipJsonObjectUtf8(ct));

    public static RoadmapSnapshotMetadata ParseMetadata(this RoadmapWorkspaceSnapshot snapshot)
        => snapshot?.Metadata?.JsonDeserializeOrDefault<RoadmapSnapshotMetadata>();
   
    public static async Task<RoadmapSnapshot> ApplyEventsToSnapshot(
        this RoadmapSnapshot snapshot,
        List<RoadmapWorkspaceEvent> events,
        CancellationToken cancellationToken)
    {
        var orderedEvents = events.OrderBy(e => e.CreatedAt).ThenBy(e => e.Version);
        var aggregator = new RoadmapSnapshotAggregator(snapshot);
        foreach (var rawEvent in orderedEvents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parsedEvent = WorkspaceEventMapper.Map(rawEvent);
            if (parsedEvent != null)
            {
                aggregator.Apply(parsedEvent);
            }
        }

        return aggregator.Build();
    }
    public static RoadmapSnapshot EmptyOnNull(this RoadmapSnapshot snapshot, string roadmapId)
        => snapshot ?? new RoadmapSnapshot()
        {
            Id = roadmapId,
            LearningItems = new List<LearningItemSnapshot>(),
            LearningItemsConnections = new List<LearningItemsConnectionSnapshot>()
        };
    public static RoadmapSnapshot MakeRoadmapSnapshot(this RoadmapDto roadmapDto)
    {
        return new RoadmapSnapshot()
        {
            Id = roadmapDto.Id,
            LearningItems = roadmapDto.Nodes?.Select(n => new LearningItemSnapshot(n.Id, n.Title, n.Description, Core.Constants.LearningStatus.NotStarted)).ToList() ?? new List<LearningItemSnapshot>(),
            LearningItemsConnections = roadmapDto.Edges?.Select(e => new LearningItemsConnectionSnapshot(e.Id, e.Source, e.Target)).ToList() ?? new List<LearningItemsConnectionSnapshot>()
        };
    }

    public static RoadmapSnapshotMetadata CalculateSnapshotMetadata(this RoadmapSnapshot snapshot)
    {
        if (snapshot == null || snapshot.LearningItems.Count == 0)
        {
            return new RoadmapSnapshotMetadata(0.0, Core.Constants.LearningStatus.NotStarted);
        }

        var totalItems = snapshot.LearningItems.DistinctBy(li => li.Id).Count();
        var completedItems = snapshot.LearningItems.Count(i => i.Status == Core.Constants.LearningStatus.Completed);
        var progress = (int)((double)completedItems / totalItems);
        var status = completedItems <= 0 ? Core.Constants.LearningStatus.NotStarted :
                     completedItems >= totalItems ? Core.Constants.LearningStatus.Completed :
                     Core.Constants.LearningStatus.InProgress;

        return new RoadmapSnapshotMetadata(progress, status);
    }
}