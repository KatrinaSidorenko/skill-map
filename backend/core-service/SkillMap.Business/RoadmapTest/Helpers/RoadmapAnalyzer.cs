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
        // 1. Build Graph & Calculate In-Degrees
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

        // 2. Calculate Levels (Longest Path) & Descendants (Impact)
        // We can compute Level during a topological sort or DFS
        var nodeLevels = new Dictionary<string, int>();
        var nodeImpacts = new Dictionary<string, int>(); // From previous answer logic

        // (Assume CalculateImpacts() and CalculateLevels() are helper methods)
        CalculateLevels(nodes, graph, inDegree, nodeLevels);
        CalculateDescendants(nodes, graph, nodeImpacts);

        // 3. Stratified Selection
        // Group by Level -> Sort by Impact -> Take Top 1
        // questions by each level based on levels count and questionsLimit
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

    // Helper to calculate Levels (BFS approach)
    private void CalculateLevels(
        List<Node> nodes,
        Dictionary<string, List<string>> graph,
        Dictionary<string, int> inDegree,
        Dictionary<string, int> levels)
    {
        var queue = new Queue<(string Id, int Level)>();

        // Start with Roots (In-Degree 0)
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
                    // Logic: A child's level is Max(current_parents) + 1
                    // Simply updating it as we traverse works for Longest Path in DAG
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
        // Cache to store the unique set of descendants for each node
        // Key: NodeId, Value: HashSet of all downstream NodeIds
        var memoizationCache = new Dictionary<string, HashSet<string>>();

        foreach (var node in nodes)
        {
            // Compute (or retrieve from cache) the full set of descendants
            var descendants = GetUniqueDescendants(node.Id, graph, memoizationCache);

            // The "Impact" is the count of these unique descendants
            nodeImpacts[node.Id] = descendants.Count;
        }
    }

    // Recursive helper method
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
