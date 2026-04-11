using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Business.RoadmapsWorkspace.Common;
public static class TopologicalSort
{
    public static bool HasCycle(List<LearningItemSnapshot> nodes, List<LearningItemsConnectionSnapshot> edges)
    {
        if (nodes == null || !nodes.Any()) return false;

        var inDegree = nodes.ToDictionary(n => n.Id, n => 0);
        var adjacencyList = nodes.ToDictionary(n => n.Id, n => new List<string>());

        foreach (var edge in edges)
        {
            if (adjacencyList.ContainsKey(edge.FromId) && inDegree.ContainsKey(edge.ToId))
            {
                adjacencyList[edge.FromId].Add(edge.ToId);
                inDegree[edge.ToId]++;
            }
        }

        var zeroInDegreeQueue = new Queue<string>(
            inDegree.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key)
        );

        int processedNodesCount = 0;

        while (zeroInDegreeQueue.Count > 0)
        {
            var currentId = zeroInDegreeQueue.Dequeue();
            processedNodesCount++;

            foreach (var neighborId in adjacencyList[currentId])
            {
                inDegree[neighborId]--;
                if (inDegree[neighborId] == 0)
                {
                    zeroInDegreeQueue.Enqueue(neighborId);
                }
            }
        }

        return processedNodesCount != nodes.Count;
    }

    public static bool HasCycle(RoadmapSnapshot snapshot)
    {
        if (snapshot == null) return false;
        return HasCycle(snapshot.LearningItems ?? [],
                        snapshot.LearningItemsConnections ?? []);
    }
}