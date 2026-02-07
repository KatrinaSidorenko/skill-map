using SkillMap.Business.Roadmaps.Helpers;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Core.Constants;
using SkillMap.Core.Entities;

namespace SkillMap.Business.ModifiedRoadmaps.Helpers;

public static class RoadmapProgressCalculator
{
    public static (double Progress, string Status) Calculate(
        IEnumerable<RoadmapModification> modifications,
        int totalTopics)
    {
        var mods = modifications.ToList();
        var createNodes = mods
            .Where(m => m.Action == ModificationAction.CreateItem)
            .Select(m => m.MapToModifiedNode())
            .ToList();
        var deletedNodeIds = mods
            .Where(m => m.Action == ModificationAction.DeleteItem)
            .Select(m => m.ExternalItemId)
            .ToHashSet();

        var createdIds = createNodes.Select(n => n.Id).ToHashSet();
        var uniqueNewNodesCount = createdIds.Except(deletedNodeIds).Count();
        var oldDeletedNodesCount = deletedNodeIds.Count(id => !createdIds.Contains(id));

        var adjustedTotal = totalTopics + uniqueNewNodesCount - oldDeletedNodesCount;
        if (adjustedTotal <= 0)
            return (0, LearningStatus.NotStarted.ToStatusString());

        var updatedItems = mods
            .Where(m => m.Action == ModificationAction.SnapshotUpdate)
            .Select(m => m.MapToChange())
            .DistinctBy(u => u.Id)
            .ToList();

        var completedCount = updatedItems.Count(n => n.Status == LearningStatus.Completed.ToStatusString());
        var progress = Math.Round((completedCount / (double)adjustedTotal), 2);

        string status;
        if (completedCount == adjustedTotal)
            status = LearningStatus.Completed.ToStatusString();
        else if (updatedItems.Any(n =>
                 n.Status == LearningStatus.InProgress.ToStatusString() ||
                 n.Status == LearningStatus.Completed.ToStatusString()))
            status = LearningStatus.InProgress.ToStatusString();
        else
            status = LearningStatus.NotStarted.ToStatusString();

        return (progress, status);
    }

    public static double CalculateRoadmapProgress(this List<ModifiedNode> nodes)
    {
        var totalNodes = nodes?.Count();
        if (nodes == null || totalNodes <= 0)
            return 0;

        var completedCount = nodes.Count(n => n.Status == LearningStatus.Completed.ToStatusString());
        var progress = Math.Round((completedCount / (double)totalNodes) * 100, 2);
        return progress;
    }

    public static string CalculateStatus(this List<ModifiedNode> nodes)
    {
        var totalNodes = nodes?.Count();
        if (nodes == null || totalNodes <= 0)
            return LearningStatus.NotStarted.ToStatusString();

        var completedCount = nodes.Count(n => n.Status == LearningStatus.Completed.ToStatusString());
        var inProgress = nodes.Any(n =>
            n.Status == LearningStatus.InProgress.ToStatusString() ||
            n.Status == LearningStatus.Completed.ToStatusString());

        if (completedCount == totalNodes)
            return LearningStatus.Completed.ToStatusString();

        return inProgress
            ? LearningStatus.InProgress.ToStatusString()
            : LearningStatus.NotStarted.ToStatusString();
    }
}