using LearningPlatform.Roadmap.Business.Contracts.Constants;

using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace.Common;

public static class RoadmapSnapshotValidator
{
    public static bool IsRoadmapStateValid(RoadmapSnapshot roadmapSnapshot)
        => !TopologicalSort.HasCycle(roadmapSnapshot);

    public static List<List<LearningItemSnapshot>> GetSCCs(List<List<LearningItemSnapshot>> sccComponents)
        => sccComponents
            .Where(c => c.Count > 1)
            .ToList();

    public static bool IsCyclic(this List<List<LearningItemSnapshot>> sccComponents)
        => sccComponents.Any(c => c.Count > 1);

    public static List<LearningItemsConnectionSnapshot> ResolveCycles(
        this List<List<LearningItemSnapshot>> sccComponents,
        List<LearningItemsConnectionSnapshot> connections)
    {
        var cycleEdges = new List<LearningItemsConnectionSnapshot>();

        foreach (var component in sccComponents)
        {
            var componentIds = component.Select(n => n.Id).ToHashSet();

            var componentEdges = connections.Where(c => componentIds.Contains(c.FromId) && componentIds.Contains(c.ToId));
            var nodeById = component.ToDictionary(n => n.Id);
            // if cycle edges between nodes of same type => remove any
            var sameTypeEdges = componentEdges.Where(c =>
            {
                var from = nodeById.GetValueOrDefault(c.FromId);
                var to = nodeById.GetValueOrDefault(c.ToId);
                return from != null && to != null && from.Type == to.Type;
            });

            if (sameTypeEdges.Any())
            {
                var edgeToRemove = sameTypeEdges.First();
                cycleEdges.Add(edgeToRemove);
                continue;
            }

            cycleEdges.AddRange(componentEdges.Where(c =>
            {
                var from = nodeById.GetValueOrDefault(c.FromId);
                var to = nodeById.GetValueOrDefault(c.ToId);
                return from == null || to == null || !IsCorrectDirection(from, to);
            }));
        }


        return cycleEdges;
    }

    private static bool IsCorrectDirection(LearningItemSnapshot from, LearningItemSnapshot to)
        => from.Type.IsTopic() && to.Type.IsSubTopic();
}