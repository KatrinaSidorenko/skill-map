using SkillMap.Business.Roadmaps.Models;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Models;
using System.Xml.Linq;

namespace SkillMap.Business.Roadmaps.Helpers;

public static class TopologicalSort
{
    public static Dictionary<string, int> GetNodesInDegree(List<NodeResponse> nodes, List<(string Source, string Target)> edges)
    {
        var inDegree = new Dictionary<string, int>();
        foreach (var node in nodes)
        {
            inDegree[node.Id] = 0;
        }

        foreach (var edge in edges)
        {
            inDegree[edge.Target]++;
        }

        return inDegree;
    }

    public static List<string> GetNodesWithZeroInDegree(Dictionary<string, int> inDegree)
        => inDegree.Where(d => d.Value <= 0).Select(d => d.Key).ToList();
    public static List<NodeDto> GetNeighbors(NodeDto node, List<EdgeDto<NodeDto>> edges)
        => edges.Where(e => e.Source.Equals(node)).Select(e => e.Target).ToList();
    public static Dictionary<string, List<string>> CreateAdjacentList(List<NodeResponse> nodes, List<(string Source, string Target)> edges)
    {
        var adjacencyList = new Dictionary<string, List<string>>();
        foreach (var node in nodes)
        {
            adjacencyList[node.Id] = new List<string>();
        }

        foreach (var edge in edges)
        {
            if (!adjacencyList.ContainsKey(edge.Source))
            {
                continue;
            }

            adjacencyList[edge.Source].Add(edge.Target);
        }

        return adjacencyList;
    }
    private static Func<NodeResponse, string> OrderRule(NodeResponse node) => new Func<NodeResponse, string>(n => n.Title); // todo: apply specific sorting rules
    public static List<NodeResponse> SortNodes(List<NodeResponse> nodes, List<(string Source, string Target)> edges)
    {
        var nodesDict = nodes.ToDictionary(n => n.Id);
        var nodesInDegrees = GetNodesInDegree(nodes, edges);
        var initialZeroInDegreeNodes = GetNodesWithZeroInDegree(nodesInDegrees).OrderBy((id) => nodesDict[id].Title);
        var adjacencyList = CreateAdjacentList(nodes, edges);

        var zeroInDegreeNodes = new Queue<string>(initialZeroInDegreeNodes);
        var result = new List<string>();
        while (zeroInDegreeNodes.Count > 0)
        {
            var node = zeroInDegreeNodes.Dequeue();
            result.Add(node);
            var neighbors = adjacencyList.GetOrDefault(node);
            if (neighbors == null || neighbors.Count < 0) { continue; }

            foreach (var neighbor in neighbors.OrderBy((id) => nodesDict[id].Title))
            {
                nodesInDegrees[neighbor]--;
                if (nodesInDegrees[neighbor] == 0)
                {
                    zeroInDegreeNodes.Enqueue(neighbor);
                }
            }
        }

        return result.Select(id => nodesDict[id]).ToList();
    }
}
