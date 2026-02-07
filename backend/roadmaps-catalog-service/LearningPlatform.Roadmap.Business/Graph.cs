using LearningPlatform.Roadmap.Business.Contracts.Models;

namespace LearningPlatform.Roadmap.Business;

public class Graph
{
    public List<NodeDto> Nodes { get; set; } = new();
    public List<EdgeDto> Edges { get; set; } = new();

    public Graph(List<NodeDto> nodes, List<EdgeDto> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public Dictionary<NodeDto, List<NodeDto>> BuildAdjacencyList()
    {
        var adjacencyList = new Dictionary<NodeDto, List<NodeDto>>();
        foreach (var node in Nodes)
        {
            adjacencyList[node] = new List<NodeDto>();
        }

        foreach (var edge in Edges)
        {
            adjacencyList[edge.Source].Add(edge.Target);
        }

        return adjacencyList;
    }

    public (List<NodeDto> Nodes, List<List<int>> AdjacencyList) ToIndexedGraph()
    {
        var nodeList = Nodes;
        var nodeToIndex = nodeList.Select((node, i) => new { node, i })
                                  .ToDictionary(x => x.node.ExternalId, x => x.i);

        var adjacencyList = new List<List<int>>(new List<int>[nodeList.Count]);
        for (int i = 0; i < nodeList.Count; i++)
        {
            adjacencyList[i] = new List<int>();
        }

        foreach (var edge in Edges)
        {
            int sourceIndex = nodeToIndex[edge.Source.ExternalId];
            int targetIndex = nodeToIndex[edge.Target.ExternalId];
            adjacencyList[sourceIndex].Add(targetIndex);
        }

        return (nodeList, adjacencyList);
    }

}