using LearningPlatform.Roadmap.Business.Contracts.Constants;
using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Business.__old.ModifiedRoadmaps.Models;
using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Business.__old.ModifiedRoadmaps.Helpers;

public static class RoadmapModificationApplier
{
    public static (List<ModifiedNode> Nodes, List<Edge> Edges) Apply(
        RoadmapDto source,
        IEnumerable<RoadmapWorkspaceEvent> modifications)
    {
        var mods = modifications.ToList();

        var createdNodes = mods
            .Where(m => m.EventType == EventType.CreateItem)
            .Select(m => m.MapToModifiedNode())
            .ToList();
        var createdConnections = mods
            .Where(m => m.EventType == EventType.CreateConnection)
            .Select(m => m.MapToLearningItemConnection())
            .ToList();

        var deletedNodeIds = mods
            .Where(m => m.EventType == EventType.DeleteItem)
            .Select(m => m.ExternalItemId)
            .ToHashSet();
        var deletedConnections = mods
            .Where(m => m.EventType == EventType.DeleteConnection)
            .Select(m => m.ExternalItemId.GetConnectionPoints())
            .ToList();

        var updatedItems = mods
            .Where(m => m.EventType == EventType.SnapshotUpdate)
            .Select(m => m.MapToChange())
            .OrderBy(u => u.CreatedAt)
            .ToList();

        var allNodes = source.Nodes.Select(n => n.MapToModifiedNode()).Concat(createdNodes).ToList();
        var nodeDict = allNodes.ToDictionary(n => n.Id, n => n);

        foreach (var update in updatedItems)
        {
            if (!nodeDict.TryGetValue(update.Id, out var node))
                continue;

            node.Title = update.Title ?? node.Title;
            node.Description = update.Description ?? node.Description;
            node.Status = update.Status ?? node.Status;
        }

        var filteredNodes = nodeDict.Values
            .Where(n => !deletedNodeIds.Contains(n.Id))
            .ToList();

        var allEdges = source.Edges.Concat(createdConnections)
            .Where(e => !deletedConnections.Contains((e.Source, e.Target)))
            .ToList();

        return (filteredNodes, allEdges);
    }
}