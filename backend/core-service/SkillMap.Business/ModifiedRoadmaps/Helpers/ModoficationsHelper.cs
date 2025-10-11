using LearningPlatform.Roadmap.Business.Contracts.Models;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Core.Constants;
using SkillMap.Core.Entities;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.Roadmaps.Helpers;

public static class ModificationsHelper
{
    public static (string TragetId, string SourceId) GetConnectionPoints(this string connection)
    {
        var parts = connection.Split("-");
        if (parts.Length != 2)
        {
            return (null, null);
        }

        return (parts[0], parts[1]);
    }
    public static ModifiedNode MapToModifiedNode(this RoadmapModification modification)
    {
        var item = modification?.Metadata.DeserializeOrDefault<LearningItem>();
        return new ModifiedNode
        {
            Id = item?.Id,
            Title = item?.Title,
            Description = item?.Description,
            Status = item?.Status?.ToLower().ToLower(),
        };
    }


    public static ModifiedNode MapToModifiedNode(this Node node)
    {
        return new ModifiedNode
        {
            Id = node?.Id,
            Title = node?.Title,
            Description = node?.Description,
            Status = LearningStatus.NotStarted.ToString().ToLower(),
        };
    }
    public static Edge MapToLearningItemConnection(this RoadmapModification modification)
    {
        var connection = modification.Metadata.DeserializeOrDefault<LearningItemConnection>();
        return new Edge
        {
            Id = modification?.ExternalItemId ?? modification?.InnerItemId,
            Source = connection?.SourceId,
            Target = connection?.TargetId,
        };
    }

    public static LearningItemChange MapToChange(this RoadmapModification modification)
    {
        var change = modification?.Metadata.DeserializeOrDefault<LearningItemChange>();
        return new LearningItemChange
        {
            Id = change?.Id,
            Title = change?.Title,
            Description = change?.Description,
            Status = change?.Status,
            CreatedAt = modification.CreatedAt,
        };
    }

    public static DeleteLearningItemChange MapToDeleteChange(this RoadmapModification modification)
    {
        var change = modification?.Metadata.DeserializeOrDefault<DeleteLearningItemChange>();
        return new DeleteLearningItemChange
        {
            Id = change?.Id,
            Type = change?.Type.ToLower().ToLower(),
        };
    }

 

    //public static List<NodeResponse> GetCreatedNodes(this IEnumerable<RoadmapModification> nodes)
    //{
    //    var createdNodes = nodes.Select(m =>
    //     {
    //         var metadata = m.Metadata?.DeserializeOrDefault<CreateLearningItemMetadata>();
    //         if (metadata == null)
    //         {
    //             return null;
    //         }

    //         var node = new NodeResponse
    //         {
    //             Id = m.InnerItemId,
    //             Title = metadata.Title,
    //             Type = metadata.Type,
    //             ParentId = metadata.ParentId,
    //             Index = int.MaxValue,
    //             Description = metadata.Description,
    //             Status = metadata.Status.ToString(),
    //             Progress = 0,
    //         };

    //         return node;
    //     })
    //    .Where(m => m != null)
    //    .ToList();
    //    createdNodes.FillChildren();

    //    return createdNodes;
    //}

    public static void ApplyModifications(this Dictionary<string, NodeResponse> nodesDict, Dictionary<ModificationAction, List<RoadmapModification>> modificationsByActions)
    {
        // item to update status
        var itemsToUpdateStatus = (modificationsByActions.GetOrDefault(ModificationAction.UpdateStatus) ?? new List<RoadmapModification>())
            .GroupBy(m => m.ExternalItemId)
            .ToDictionary(m => m.Key, m => m.Select(t => new
            {
                t.UpdatedAt,
                t.ExternalItemId,
                Metadata = t.Metadata?.DeserializeOrDefault<UpdateStatusMetadata>(),
            }).OrderByDescending(m => m.UpdatedAt).FirstOrDefault());

        // todo: clever logic for for status and progress handling
        // if at least one children is in progress, set parent to in progress
        // if all children are completed, set parent to completed, progress to 100
        // if all children are not started, set parent to not started, progress to 0
        var snapshotUpdates = (modificationsByActions.GetOrDefault(ModificationAction.SnapshotUpdate) ?? new List<RoadmapModification>())
            .Select(m => new {
                m.UpdatedAt,
                Metadata = m.Metadata?.DeserializeOrDefault<LearningItemChange>()
            })
            .Where(m => m != null)
            .GroupBy(m => m.Metadata?.Id)
            .ToDictionary(m => m.Key, m => m.Select(t => new
            {
                t.UpdatedAt,
                t.Metadata,
            }).OrderByDescending(m => m.UpdatedAt).FirstOrDefault());

        var allChangedItems = itemsToUpdateStatus
            .Select(i => i.Key)
            .Union(snapshotUpdates.Select(i => i.Key))
            .ToList();

        foreach (var itemId in allChangedItems)
        {
            var node = nodesDict.GetOrDefault(itemId);
            if (node == null)
            {
                continue;
            }

            var snapshotUpdate = snapshotUpdates.GetOrDefault(itemId);

            var snapshot = snapshotUpdate?.Metadata;
            var snapshotDate = snapshotUpdate?.UpdatedAt;

            var targetStatus = node.Status;
            var status = snapshotUpdate?.Metadata?.Status;
            var statusPartialUpdate = itemsToUpdateStatus.GetOrDefault(itemId);
            // todo: fix status logic and progress calculations 
            if (statusPartialUpdate?.UpdatedAt != null || statusPartialUpdate?.UpdatedAt > snapshotDate)
            {
                targetStatus = statusPartialUpdate?.Metadata?.Status.ToString();
            }
            else
            {
                targetStatus = snapshot?.Status.ToString();
            }

            if (targetStatus != null)
            {
                node.Status = targetStatus;
                if (targetStatus == LearningStatus.Completed.ToString())
                {
                    node.Progress = 100;
                }
            }

            var targetTitle = node.Title;
            var title = snapshot?.Title;
            node.Title = title ?? targetTitle;

            var targetDescription = node.Description;
            var description = snapshot?.Description;
            node.Description = description ?? targetDescription;
        }
    }

    //public static (List<NodeResponse>, List<(string Source, string Target)>) GetTreeWithModifications(this (List<NodeResponse>, List<(string Source, string Target)>) sourceRoadmap, List<RoadmapModification> modifications)
    //{
    //    var createdNodesModifications = modifications
    //       .Where(m => m.Action == ModificationAction.Create)
    //       .GetCreatedNodes();

    //    var (createdNodes, createdEdges) = (new List<NodeResponse>(), new Dictionary<string, List<string>>()).ToGraph(createdNodesModifications);

    //    var (nodes, edges) = sourceRoadmap;
    //    var rootNode = nodes.FirstOrDefault(n => n.Type.IsRoadmap());
    //    nodes.AddRange(createdNodes);

    //    var plainEdges = createdEdges
    //        .SelectMany(e => e.Value.Select(t => (Source: e.Key, Target: t)))
    //        .ToList();
    //    edges.AddRange(plainEdges);

    //    var nodesDictionary = nodes.ToDictionary(n => n.Id, n => n);

    //    var modificationsByActions = modifications
    //        .GroupBy(m => m.Action)
    //        .ToDictionary(g => g.Key, g => g.ToList());
    //    nodesDictionary.ApplyModifications(modificationsByActions);

    //    var topicsAndSubtopics = nodes.Where(n => !n.Type.IsResource()).ToList();
    //    var targetEdges = edges
    //        .Where(e => topicsAndSubtopics.Any(n => n.Id == e.Source) && topicsAndSubtopics.Any(n => n.Id == e.Target))
    //        .ToList();
    //    var sortedNodes = TopologicalSort.SortNodes(topicsAndSubtopics, targetEdges);
    //    var sortedNodesIndexing = sortedNodes
    //        .Select((n, i) => new
    //        {
    //            n.Id,
    //            Index = i,
    //        })
    //        .ToDictionary(n => n.Id, n => (int?)n.Index);
    //    nodes.ApplyIndexing(sortedNodesIndexing);

    //    var itemsToDelete = (modificationsByActions.GetOrDefault(ModificationAction.Delete) ?? new List<RoadmapModification>())
    //     .Select(m => m.ExternalItemId)
    //     .ToList();

    //    nodes = nodes
    //        .Where(n => !itemsToDelete.Contains(n.Id))
    //        .ToList();

    //    edges = edges
    //        .Where(e => !itemsToDelete.Contains(e.Source) && !itemsToDelete.Contains(e.Target))
    //        .ToList();


    //    return (nodes, edges);
    //}

    //public static (List<NodeResponse>, List<(string Source, string Target)>) ApplyModifications(this (List<NodeResponse>, List<(string Source, string Target)>) sourceRoadmap, List<RoadmapModification> modifications)
    //{
    //    var createdNodesModifications = modifications
    //       .Where(m => m.Action == ModificationAction.Create)
    //       .GetCreatedNodes();

    //    var (createdNodes, createdEdges) = (new List<NodeResponse>(), new Dictionary<string, List<string>>()).ToGraph(createdNodesModifications);

    //    var (nodes, edges) = sourceRoadmap;
    //    var rootNode = nodes.FirstOrDefault(n => n.Type.IsRoadmap());
    //    nodes.AddRange(createdNodes);

    //    var plainEdges = createdEdges
    //        .SelectMany(e => e.Value.Select(t => (Source: e.Key, Target: t)))
    //        .ToList();
    //    edges.AddRange(plainEdges);

    //    var nodesDictionary = nodes.ToDictionary(n => n.Id, n => n);

    //    var modificationsByActions = modifications
    //        .GroupBy(m => m.Action)
    //        .ToDictionary(g => g.Key, g => g.ToList());
    //    nodesDictionary.ApplyModifications(modificationsByActions);

    //    var itemsToDelete = (modificationsByActions.GetOrDefault(ModificationAction.Delete) ?? new List<RoadmapModification>())
    //     .Select(m => m.ExternalItemId)
    //     .ToList();

    //    nodes.ForEach(n =>
    //    {
    //        if (itemsToDelete.Contains(n.Id))
    //        {
    //            n.IsDeleted = true;
    //        }
    //    });

    //    //edges = edges
    //    //    .Where(e => !itemsToDelete.Contains(e.Source) && !itemsToDelete.Contains(e.Target))
    //    //    .ToList();


    //    return (nodes, edges);
    //}

    //public static void ApplySortingAndIndexing(this (List<NodeResponse>, List<(string Source, string Target)>) sourceRoadmap)
    //{
    //    var (nodes, edges) = sourceRoadmap;
    //    var topicsAndSubtopics = nodes.Where(n => !n.Type.IsResource()).ToList();
    //    var targetEdges = edges
    //        .Where(e => topicsAndSubtopics.Any(n => n.Id == e.Source) && topicsAndSubtopics.Any(n => n.Id == e.Target))
    //        .ToList();
    //    var sortedNodes = TopologicalSort.SortNodes(topicsAndSubtopics, targetEdges);
    //    var sortedNodesIndexing = sortedNodes
    //        .Where(n => !n.IsDeleted)
    //        .Select((n, i) => new
    //        {
    //            n.Id,
    //            Index = i,
    //        })
    //        .ToDictionary(n => n.Id, n => (int?)n.Index);

    //    nodes.ApplyIndexing(sortedNodesIndexing);
    //}
}
