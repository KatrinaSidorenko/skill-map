using SkillMap.Business.Roadmaps.Models;
using SkillMap.Shared;
using SkillMap.Shared.Extensions;
using System.Runtime.CompilerServices;

namespace SkillMap.Business.Roadmaps.Helpers;

public static class TreeResponseExtensions
{
    public static (List<NodeResponse> Nodes, Dictionary<string, List<string>> Edges) ToGraph(this TreeResponse treeResponse)
    {
        var nodes = new List<NodeResponse>();
        var edges = new Dictionary<string, List<string>>();

        nodes.Add(new NodeResponse
        {
            Id = treeResponse.Id,
            Title = treeResponse.Title,
            Type = "roadmap"
        });

        return (nodes, edges).ToGraph(treeResponse.Nodes);
    }

    public static (List<NodeResponse> Nodes, Dictionary<string, List<string>> Edges) ToGraph(
        this (List<NodeResponse> Nodes, Dictionary<string, List<string>> Edges) graph,
        List<NodeResponse> nodes)
    {
        foreach (var node in nodes)
        {
            if (!graph.Nodes.Any(n => n.Id == node.Id))
            {
                graph.Nodes.Add(node);
            }

            if (!string.IsNullOrEmpty(node.ParentId))
            {
                if (!graph.Edges.ContainsKey(node.ParentId))
                {
                    graph.Edges[node.ParentId] = new List<string>();
                }

                if (!graph.Edges[node.ParentId].Contains(node.Id))
                {
                    graph.Edges[node.ParentId].Add(node.Id);
                }
               
            }

            graph = graph.ToGraph(node.Children);
        }

        return graph;
    }

    public static (List<NodeResponse> Nodes, List<(string Source, string Target)> Edges) ToPlainGraph(this TreeResponse treeResponse)
    {
        var nodes = new List<NodeResponse>();
        nodes.Add(new NodeResponse
        {
            Id = treeResponse.Id,
            Title = treeResponse.Title,
            Type = NodeType.Roadmap,
            Children = treeResponse.Nodes,
            Index = -1
        }) ;

        nodes.AddRange(treeResponse.Nodes);

        var initialEdges = treeResponse.Nodes
            .Select(c => (Source: treeResponse.Id, Target: c.Id))
            .ToList();

        var edges = new List<(string Source, string Target)>(initialEdges);
        treeResponse.Nodes.ForEach(node => node.ToNodeEdge(nodes, edges));

        return (nodes.DistinctBy(n => n.Id).ToList(), edges.GroupBy(e => e).Select(g => g.FirstOrDefault()).ToList());
    }

    private static void ToNodeEdge(this NodeResponse node, List<NodeResponse> nodes, List<(string Source, string Target)> edges)
    {
        nodes.Add(node);

        foreach (var child in node.Children)
        {
            if (!string.IsNullOrEmpty(node.Id))
            {
                edges.Add((node.Id, child.Id));
            }

            child.ToNodeEdge(nodes, edges);
        }
    }

    public static TreeResponse ToTree(this (List<NodeResponse> Nodes, List<(string Source, string Target)> Edges) graph)
    {
        var nodesDict = graph.Nodes.DistinctBy(n => n.Id).ToDictionary(n => n.Id);
        var edgeLookup = graph.Edges
            .GroupBy(e => e.Source)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Target).ToList());

        foreach (var node in graph.Nodes)
        {
            node.Children = edgeLookup.TryGetValue(node.Id, out var targets)
                ? targets.Select(t => nodesDict[t]).ToList()
                : new List<NodeResponse>();
        }

        var rootNode = graph.Nodes.FirstOrDefault(n => n.Type.IsRoadmap());
        var topics = graph.Nodes.Where(n => n.Type.IsTopic())
            .ToList();

        return new TreeResponse
        {
            Id = rootNode?.Id ?? "",
            Title = rootNode?.Title ?? "",
            Nodes = topics
        };
    }

    public static void FillChildren(this (List<NodeResponse> Nodes, List<(string Source, string Target)> Edges) graph)
    {
        var nodesDict = graph.Nodes.DistinctBy(n => n.Id).ToDictionary(n => n.Id);
        var edgeLookup = graph.Edges
            .GroupBy(e => e.Source)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Target).ToList());

        foreach (var node in graph.Nodes)
        {
            node.Children = edgeLookup.TryGetValue(node.Id, out var targets)
                ? targets.Select(t => nodesDict[t]).Where(n => !n.IsDeleted).ToList()
                : new List<NodeResponse>();
        }
    }

    public static void FillChildren(this List<NodeResponse> nodes)
    {
        // children inside of nodes
        var innerTree = nodes.GroupBy(n => n.ParentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var node in nodes)
        {
            if (innerTree.TryGetValue(node.Id, out var children))
            {
                node.Children = children;
            }
            else
            {
                node.Children = new List<NodeResponse>();
            }
        }
    }

    public static void ApplyIndexing(this List<NodeResponse> nodes, Dictionary<string, int?> indexes)
    {
        foreach (var node in nodes)
        {
            var index = indexes.GetOrDefault(node.Id) ?? -1;
            node.Index = index;

            node.Children?.ApplyIndexing(indexes);
        }
    }

    public static List<CustomizedUserRoadmapLearningItem> GetAllFlattenNodes(this List<CustomizedUserRoadmapLearningItem> treeResponse)
    {
        var nodes = new List<CustomizedUserRoadmapLearningItem>();

        foreach (var child in treeResponse)
        {
            nodes.AddRange(child.GetAllNodes());
        }

        return nodes;
    }

    private static List<CustomizedUserRoadmapLearningItem> GetAllNodes(this CustomizedUserRoadmapLearningItem node)
    {
        var nodes = new List<CustomizedUserRoadmapLearningItem> { node };

        foreach (var child in node.Children)
        {
            nodes.AddRange(child.GetAllNodes());
        }

        return nodes;
    }
}

