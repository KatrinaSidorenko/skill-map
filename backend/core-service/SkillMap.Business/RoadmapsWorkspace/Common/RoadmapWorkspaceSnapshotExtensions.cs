using System.Threading;

using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;

using MediatR;

using Microsoft.Extensions.Logging;

using SkillMap.Business.RoadmapsWorkspace.Common;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Gzip;

namespace SkillMap.Business.PersonalizedRoadmaps.Common;
public static class RoadmapWorkspaceSnapshotExtensions
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
            LearningItems = roadmapDto.Nodes?.Select(n => new LearningItemSnapshot(n.Id, n.Title, n.Description, n.Type, Core.Constants.LearningStatus.NotStarted)).ToList() ?? new List<LearningItemSnapshot>(),
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
        return CalculateSnapshotMetadata(totalItems, completedItems);
    }

    public static RoadmapSnapshotMetadata CalculateSnapshotMetadata(int totalItems, int completedItems)
    {
        var progress = totalItems > 0 ? ((double)completedItems / totalItems) : 0;
        var status = completedItems <= 0 ? Core.Constants.LearningStatus.NotStarted :
                     completedItems >= totalItems ? Core.Constants.LearningStatus.Completed :
                     Core.Constants.LearningStatus.InProgress;

        return new RoadmapSnapshotMetadata(progress, status);
    }

    public static async Task<RoadmapWorkspaceSnapshot> CreateRoadmapWorkspaceSnapshot(
        long workspaceId,
        RoadmapWorkspaceSnapshot latestSnapshot,
        List<RoadmapWorkspaceEvent> eventsList,
        CancellationToken ct)
    {
        var snapshotContent = await latestSnapshot.GetRoadmapSnapshot(ct);
        var newRoadmapSnapshot = await snapshotContent.ApplyEventsToSnapshot(eventsList, ct);

        var newSnapshot = new RoadmapWorkspaceSnapshot(workspaceId);
        newSnapshot.SetVersion(eventsList.OrderByDescending(e => e.Version).First().Version);
        newSnapshot.SetMetadata(newRoadmapSnapshot.CalculateSnapshotMetadata());
        await newSnapshot.SetRoadmapSnapshot(newRoadmapSnapshot, ct);
        return newSnapshot;
    }

    public static CreateRoadmapDto ToCreateBlueprint(this
        RoadmapSnapshot roadmapSnapshot,
        string title,
        string description,
        string imageUrl,
        long authorId, int version)
    {
        var nodes = roadmapSnapshot.LearningItems.Select(li => new NodeDto
        {
            Id = li.Id,
            Title = li.Title,
            Description = li.Description,
            Type = NodeType.Topic,
        }).DistinctBy(li => li.Id).ToDictionary(li => li.Id, li => li);

        return new CreateRoadmapDto
        {
            SourceId = roadmapSnapshot.Id,
            OwnerId = authorId.ToString(),
            SourceVersion = version,
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            Nodes = nodes.Values.ToList(),
            Edges = roadmapSnapshot.LearningItemsConnections.Select(lc => new EdgeDto
            {
                Id = lc.Id,
                Source = nodes.GetOrDefault(lc.FromId),
                Target = nodes.GetOrDefault(lc.ToId),
            }).ToList(),
        };
    }

    public static RoadmapSnapshot EnsureNoCycleEdges(this RoadmapSnapshot snapshot, ILogger logger)
    {
        var sccComponents = new TarjanSccDetector(snapshot).FindStronglyConnectedComponents();

        if (!sccComponents.IsCyclic())
            return snapshot;

        logger.LogWarning(
                   "Roadmap blueprint {RoadmapId} has cyclic dependencies — attempting to resolve via Tarjan SCC",
                      snapshot.Id);

        var cyclicSccs = RoadmapSnapshotValidator.GetSCCs(sccComponents);
        var edgesToRemove = cyclicSccs.ResolveCycles(snapshot.LearningItemsConnections);
        var edgeIdsToRemove = edgesToRemove.Select(e => e.Id).ToHashSet();

        logger.LogWarning(
            "Removing {Count} cycle edge(s) from roadmap {RoadmapId}: [{Edges}]",
            edgeIdsToRemove.Count,
            snapshot.Id,
            string.Join(", ", edgeIdsToRemove));

        return new RoadmapSnapshot
        {
            Id = snapshot.Id,
            Version = snapshot.Version,
            LearningItems = snapshot.LearningItems,
            LearningItemsConnections = snapshot.LearningItemsConnections.Where(c => !edgeIdsToRemove.Contains(c.Id)).ToList()
        };
    }
}