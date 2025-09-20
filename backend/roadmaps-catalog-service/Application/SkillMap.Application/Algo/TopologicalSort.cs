using SkillMap.Application.Domain;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;

namespace SkillMap.Application;

public static class TopologicalSort
{
    public static Dictionary<NodeDto, int> GetNodesInDegree(List<NodeDto> nodes, List<EdgeDto<NodeDto>> edges)
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
    public static List<NodeDto> GetNeighbors(NodeDto node, List<EdgeDto<NodeDto>> edges)
        => edges.Where(e => e.Source.Equals(node)).Select(e => e.Target).ToList();
    public static Dictionary<NodeDto, List<NodeDto>> CreateAdjacentList(List<NodeDto> nodes, List<EdgeDto<NodeDto>> edges)
        => new Graph(nodes, edges).BuildAdjacencyList();
    private static Func<NodeDto, string> OrderRule(NodeDto node) => new Func<NodeDto, string>(n => n.Title); // todo: apply specific sorting rules

    public static List<NodeDto> SortNodes(List<NodeDto> nodes, List<EdgeDto<NodeDto>> edges)
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
