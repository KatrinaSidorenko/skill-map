using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Core.Constants;

namespace SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;

/// <summary>
/// Selects subtopics for the initial assessment by stratifying the topic graph into levels
/// and picking the highest-impact topics from each level (breadth-first, greedy by descendant count),
/// then resolving their child subtopics as the actual question candidates.
/// </summary>
internal static class StratifiedRoadmapTopicsSelector
{
    internal static List<LeaningItemAssessment> SelectCoreSubtopics(
        List<LeaningItemAssessment> subtopicPool,
        List<LeaningItemAssessment> topics,
        List<LearningItemsConnectionAssessment> topicDependencies,
        Dictionary<string, List<string>> subtopicToTopicMap,
        int budget,
        Random rnd)
    {
        if (budget <= 0) return [];

        // 1. Rank topics by graph position and pick the best ones
        var bestTopicIds = SelectBestTopicIds(topics, topicDependencies, budget);

        // 2. Collect subtopics whose parent topic is among the best-ranked ones
        var fromBestTopics = subtopicPool
            .Where(s => subtopicToTopicMap.TryGetValue(s.Id, out var parentTopics) &&
                parentTopics.Any(t => bestTopicIds.Contains(t)))
            .OrderBy(_ => rnd.Next())
            .ToList();

        var selected = fromBestTopics.Take(budget).ToList();

        // 3. Fallback: fill remaining slots with random subtopics from the full pool
        if (selected.Count < budget)
        {
            var remaining = subtopicPool
                .Except(selected)
                .OrderBy(_ => rnd.Next())
                .Take(budget - selected.Count);

            selected.AddRange(remaining);
        }

        return selected;
    }

    // -------------------------------------------------------------------------
    // Graph-based topic ranking
    // -------------------------------------------------------------------------

    private static HashSet<string> SelectBestTopicIds(
        List<LeaningItemAssessment> topics,
        List<LearningItemsConnectionAssessment> topicDependencies,
        int budget)
    {
        if (topics.Count == 0) return [];

        var graph = new Dictionary<string, List<string>>();
        var inDegree = topics.ToDictionary(n => n.Id, _ => 0);

        foreach (var topic in topics) graph[topic.Id] = [];

        foreach (var edge in topicDependencies)
        {
            if (!graph.ContainsKey(edge.FromId) || !graph.ContainsKey(edge.ToId)) continue;
            graph[edge.FromId].Add(edge.ToId);
            inDegree[edge.ToId]++;
        }

        var nodeLevels = new Dictionary<string, int>();
        var nodeImpacts = new Dictionary<string, int>();

        CalculateLevels(topics, graph, inDegree, nodeLevels);
        CalculateDescendants(topics, graph, nodeImpacts);

        if (nodeLevels.Count == 0) return topics.Take(budget).Select(t => t.Id).ToHashSet();

        var levelsCount = nodeLevels.Values.Max() + 1;
        var topicsPerLevel = Math.Max(1, budget / levelsCount);

        return topics
            .Select(t => new
            {
                Id = t.Id,
                Level = nodeLevels.GetValueOrDefault(t.Id, 0),
                Impact = nodeImpacts.GetValueOrDefault(t.Id, 0)
            })
            .GroupBy(x => x.Level)
            .OrderBy(g => g.Key)
            .SelectMany(group => group
                .OrderByDescending(x => x.Impact)
                .Take(topicsPerLevel)
                .Select(x => x.Id))
            .Take(budget)
            .ToHashSet();
    }

    private static void CalculateLevels(
        List<LeaningItemAssessment> topics,
        Dictionary<string, List<string>> graph,
        Dictionary<string, int> inDegree,
        Dictionary<string, int> levels)
    {
        var queue = new Queue<(string Id, int Level)>();

        foreach (var topic in topics.Where(n => inDegree[n.Id] == 0))
        {
            queue.Enqueue((topic.Id, 0));
            levels[topic.Id] = 0;
        }

        while (queue.Count > 0)
        {
            var (currentId, currentLevel) = queue.Dequeue();

            if (!graph.TryGetValue(currentId, out var children)) continue;

            foreach (var child in children)
            {
                int nextLevel = currentLevel + 1;
                if (!levels.TryGetValue(child, out var existingLevel) || existingLevel < nextLevel)
                {
                    levels[child] = nextLevel;
                    queue.Enqueue((child, nextLevel));
                }
            }
        }
    }

    private static void CalculateDescendants(
        List<LeaningItemAssessment> topics,
        Dictionary<string, List<string>> graph,
        Dictionary<string, int> nodeImpacts)
    {
        var cache = new Dictionary<string, HashSet<string>>();

        foreach (var topic in topics)
            nodeImpacts[topic.Id] = GetUniqueDescendants(topic.Id, graph, cache).Count;
    }

    private static HashSet<string> GetUniqueDescendants(
        string nodeId,
        Dictionary<string, List<string>> graph,
        Dictionary<string, HashSet<string>> cache)
    {
        if (cache.TryGetValue(nodeId, out var cached)) return cached;

        var descendants = new HashSet<string>();

        if (graph.TryGetValue(nodeId, out var children))
        {
            foreach (var childId in children)
            {
                descendants.Add(childId);
                descendants.UnionWith(GetUniqueDescendants(childId, graph, cache));
            }
        }

        cache[nodeId] = descendants;
        return descendants;
    }
}