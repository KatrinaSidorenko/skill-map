using LearningPlatform.Roadmap.Business.Contracts.Models;

using SkillMap.Shared.Extensions;

namespace LearningPlatform.Roadmap.Business.Algo;

public static class TopologicalSort
{
    public static Dictionary<NodeDto, int> GetNodesInDegree(List<NodeDto> nodes, List<EdgeDto> edges)
    {
        var inDegree = new Dictionary<NodeDto, int>();
        foreach (var node in nodes)
        {
            inDegree[node] = 0;
        }

        foreach (var edge in edges)
        {
            if (inDegree.ContainsKey(edge.Target))
            {
                inDegree[edge.Target]++;
            }
        }

        return inDegree;
    }

    public static List<NodeDto> GetNodesWithZeroInDegree(Dictionary<NodeDto, int> inDegree)
        => inDegree.Where(d => d.Value <= 0).Select(d => d.Key).ToList();
    public static List<NodeDto> GetNeighbors(NodeDto node, List<EdgeDto> edges)
        => edges.Where(e => e.Source.Equals(node)).Select(e => e.Target).ToList();
    public static Dictionary<NodeDto, List<NodeDto>> CreateAdjacentList(List<NodeDto> nodes, List<EdgeDto> edges)
        => new Graph(nodes, edges).BuildAdjacencyList();

    public static List<NodeDto> SortNodes(List<NodeDto> nodes, List<EdgeDto> edges)
    {
        var nodesInDegrees = GetNodesInDegree(nodes, edges);
        var initialZeroInDegreeNodes = GetNodesWithZeroInDegree(nodesInDegrees).OrderBy(n => n.Title).ToList();
        var adjacencyList = CreateAdjacentList(nodes, edges);

        var zeroInDegreeNodes = new Queue<NodeDto>(initialZeroInDegreeNodes);
        var result = new List<NodeDto>();
        while (zeroInDegreeNodes.Count > 0)
        {
            var node = zeroInDegreeNodes.Dequeue();
            result.Add(node);
            var neighbors = adjacencyList.GetOrDefault(node);
            if (neighbors == null || neighbors.Count < 0) { continue; }

            var orderedNeighbors = neighbors.OrderBy(n => n.Title).ToList();
            foreach (var neighbor in orderedNeighbors)
            {
                nodesInDegrees[neighbor]--;
                if (nodesInDegrees[neighbor] == 0)
                {
                    zeroInDegreeNodes.Enqueue(neighbor);
                }
            }
        }

        return result;
    }
}