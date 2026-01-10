using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts.Models;

namespace SkillMap.Business.RoadmapTest.Helpers;

public class RoadmapAnalyzer
{
    public List<Topic> SelectStratifiedCoreTopics(
        List<Node> nodes,
        List<Edge> edges,
        int questionsLimit)
    {
        var graph = new Dictionary<string, List<string>>();
        var inDegree = nodes.ToDictionary(n => n.Id, n => 0);

        foreach (var node in nodes) graph[node.Id] = new List<string>();
        foreach (var edge in edges)
        {
            if (edge.Source == null || edge.Target == null) continue;
            if (!graph.ContainsKey(edge.Source) || !graph.ContainsKey(edge.Target)) continue;
            graph[edge.Source].Add(edge.Target);
            if (inDegree.ContainsKey(edge.Target)) inDegree[edge.Target]++;
        }

        var nodeLevels = new Dictionary<string, int>();
        var nodeImpacts = new Dictionary<string, int>(); 

        CalculateLevels(nodes, graph, inDegree, nodeLevels);
        CalculateDescendants(nodes, graph, nodeImpacts);

        var levelsCount = nodeLevels.Values.Max() + 1;
        var questionsPerLevel = Math.Max(1, questionsLimit / levelsCount);

        return nodes
            .Select(n => new
            {
                Node = n,
                Level = nodeLevels.ContainsKey(n.Id) ? nodeLevels[n.Id] : 0,
                Impact = nodeImpacts.ContainsKey(n.Id) ? nodeImpacts[n.Id] : 0
            })
            .GroupBy(x => x.Level)
            .OrderBy(g => g.Key) // Start from basics (Level 0) going up
            .SelectMany(group => group
                .OrderByDescending(x => x.Impact) // Prioritize "Core" topics
                .Take(questionsPerLevel) // Take top 2 "Pillars" from this level
                .Select(x => x.Node)
            )
            .Take(questionsLimit) // Hard cap
            .Select(n => new Topic(n.Id, n.Title, n.Description)).ToList();
    }

    private void CalculateLevels(
        List<Node> nodes,
        Dictionary<string, List<string>> graph,
        Dictionary<string, int> inDegree,
        Dictionary<string, int> levels)
    {
        var queue = new Queue<(string Id, int Level)>();

        foreach (var node in nodes.Where(n => inDegree[n.Id] == 0))
        {
            queue.Enqueue((node.Id, 0));
            levels[node.Id] = 0;
        }

        while (queue.Count > 0)
        {
            var (currentId, currentLevel) = queue.Dequeue();

            if (graph.TryGetValue(currentId, out var children))
            {
                foreach (var child in children)
                {
                    int nextLevel = currentLevel + 1;
                    if (!levels.ContainsKey(child) || levels[child] < nextLevel)
                    {
                        levels[child] = nextLevel;
                        queue.Enqueue((child, nextLevel));
                    }
                }
            }
        }
    }

    private void CalculateDescendants(
    List<Node> nodes,
    Dictionary<string, List<string>> graph,
    Dictionary<string, int> nodeImpacts)
    {
        var memoizationCache = new Dictionary<string, HashSet<string>>();

        foreach (var node in nodes)
        {
            var descendants = GetUniqueDescendants(node.Id, graph, memoizationCache);

            nodeImpacts[node.Id] = descendants.Count;
        }
    }

    private HashSet<string> GetUniqueDescendants(
        string nodeId,
        Dictionary<string, List<string>> graph,
        Dictionary<string, HashSet<string>> cache)
    {
        // 1. Check Cache (Memoization)
        if (cache.TryGetValue(nodeId, out var cachedResult))
        {
            return cachedResult;
        }

        // 2. Initialize set for this node
        var myDescendants = new HashSet<string>();

        // 3. Traverse Children
        if (graph.TryGetValue(nodeId, out var children))
        {
            foreach (var childId in children)
            {
                // Add the child itself
                myDescendants.Add(childId);

                // Recursively get the child's descendants
                var childDescendants = GetUniqueDescendants(childId, graph, cache);

                // Add all of the child's descendants to my set
                myDescendants.UnionWith(childDescendants);
            }
        }

        // 4. Save to Cache and Return
        cache[nodeId] = myDescendants;
        return myDescendants;
    }
}
