using System.Linq;

using SkillMap.Core.Constants;

namespace SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;

/// <summary>
/// Selects frontier subtopics using a graph-based, structurally-aware greedy set-cover algorithm.
/// Topics are analysed for their depth, ancestor/descendant reachability and structural influence,
/// then the most informative frontier topics are chosen so that their associated subtopics are returned.
/// </summary>
internal static class FrontierSubtopicSelector
{
    internal static List<LeaningItemAssessment> Select(
        List<LeaningItemAssessment> frontierPool,
        List<LeaningItemAssessment> topics,
        List<LearningItemsConnectionAssessment> topicDependencies,
        Dictionary<string, List<string>> subtopicToTopicMap,
        int budget,
        Random rnd)
    {
        if (budget <= 0) return [];

        var initiallyMasteredTopicIds = topics
            .Where(t => t.Status == LearningStatus.Completed || t.Assumption == AssessmentAssumption.AssumedCompleted)
            .Select(t => t.Id)
            .ToHashSet();

        var structureMeta = AnalyzeStructure(new AssessmentGraph { Nodes = topics, Edges = topicDependencies });
        var bestFrontierTopicIds = RunGreedySetCover(structureMeta, budget, initiallyMasteredTopicIds);

        // Gather frontier subtopics belonging to the elected topics
        var fromBestTopics = frontierPool
            .Where(s => subtopicToTopicMap.TryGetValue(s.Id, out var parentTopics) && parentTopics.Any(t => bestFrontierTopicIds.Contains(t)))
            .OrderBy(_ => rnd.Next())
            .ToList();

        var selected = fromBestTopics.Take(budget).ToList();

        // Fallback: pad with remaining random frontier subtopics when graph coverage falls short
        if (selected.Count < budget)
        {
            var remaining = frontierPool
                .Except(selected)
                .OrderBy(_ => rnd.Next())
                .Take(budget - selected.Count);

            selected.AddRange(remaining);
        }

        return selected;
    }

    // -------------------------------------------------------------------------
    // Graph structure analysis
    // -------------------------------------------------------------------------

    private static Dictionary<string, NodeMeta> AnalyzeStructure(AssessmentGraph graph)
    {
        var meta = BuildMetadata(graph);
        ComputeDepths(meta, graph.Edges.ToList());
        ComputeReachability(meta, graph.Edges.ToList());
        ComputeStructuralInfluence(meta);
        return meta;
    }

    private static Dictionary<string, NodeMeta> BuildMetadata(AssessmentGraph graph)
    {
        var map = graph.Nodes.ToDictionary(n => n.Id, n => new NodeMeta(n));

        foreach (var edge in graph.Edges)
        {
            if (!map.ContainsKey(edge.FromId) || !map.ContainsKey(edge.ToId)) continue;
            map[edge.ToId].InDegree++;
            map[edge.FromId].OutDegree++;
        }

        return map;
    }

    private static void ComputeDepths(Dictionary<string, NodeMeta> meta, List<LearningItemsConnectionAssessment> edges)
    {
        var queue = new Queue<NodeMeta>(meta.Values.Where(n => n.InDegree == 0));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var edge in edges.Where(e => e.FromId == current.Node.Id))
            {
                var target = meta[edge.ToId];
                target.Depth = Math.Max(target.Depth, current.Depth + 1);
                target.InDegree--;

                if (target.InDegree == 0) queue.Enqueue(target);
            }
        }
    }

    private static void ComputeReachability(Dictionary<string, NodeMeta> meta, List<LearningItemsConnectionAssessment> edges)
    {
        var forward = edges.GroupBy(e => e.FromId).ToDictionary(g => g.Key, g => g.Select(e => e.ToId).ToList());
        var backward = edges.GroupBy(e => e.ToId).ToDictionary(g => g.Key, g => g.Select(e => e.FromId).ToList());

        foreach (var node in meta.Values)
        {
            DFS(node.Node.Id, node.Descendants, forward);
            DFS(node.Node.Id, node.Ancestors, backward);
        }
    }

    private static void DFS(string start, HashSet<string> visited, Dictionary<string, List<string>> adjacency)
    {
        if (!adjacency.TryGetValue(start, out var neighbors)) return;

        foreach (var neighbor in neighbors)
        {
            if (visited.Add(neighbor)) DFS(neighbor, visited, adjacency);
        }
    }

    private static void ComputeStructuralInfluence(Dictionary<string, NodeMeta> meta)
    {
        foreach (var node in meta.Values)
            node.StructuralInfluence = node.Ancestors.Count + node.Descendants.Count;
    }

    // -------------------------------------------------------------------------
    // Greedy weighted set-cover across depth layers
    // -------------------------------------------------------------------------

    private static List<string> RunGreedySetCover(
   Dictionary<string, NodeMeta> meta,
     int budget,
      HashSet<string> initiallyMastered)
    {
        var selected = new HashSet<string>();
        var covered = new HashSet<string>();
        var mastered = new HashSet<string>(initiallyMastered);

        // Pre-cover all nodes reachable from already-mastered topics
        foreach (var mId in mastered)
        {
            if (meta.TryGetValue(mId, out var mNode))
            {
                covered.Add(mId);
                covered.UnionWith(mNode.Ancestors);
                covered.UnionWith(mNode.Descendants);
            }
        }

        var layers = meta.Values
            .Where(n => !mastered.Contains(n.Node.Id))
            .GroupBy(n => n.Depth)
            .OrderBy(g => g.Key)
            .Select(g => g.ToList())
            .ToList();

        if (layers.Count == 0) return selected.ToList();

        // Proportionally allocate budget across layers by average structural influence
        var layerWeights = layers.Select(l => l.Average(n => n.StructuralInfluence)).ToList();
        var allocations = AllocateBudgetByWeight(layerWeights, budget);

        // Per-layer greedy selection
        for (int i = 0; i < layers.Count; i++)
        {
            int layerBudget = allocations[i];
            if (layerBudget == 0) continue;

            int selectedInLayer = 0;
            while (selectedInLayer < layerBudget && selected.Count < budget)
            {
                var best = PickBest(layers[i], selected, mastered, covered);
                if (best is null) break;

                Commit(best, selected, covered, mastered);
                selectedInLayer++;
            }
        }

        // Global fallback: fill any remaining budget from unreachable nodes
        while (selected.Count < budget)
        {
            var best = PickBest(meta.Values, selected, mastered, covered);
            if (best is null) break;

            Commit(best, selected, covered, mastered);
        }

        return selected.ToList();
    }

    private static List<int> AllocateBudgetByWeight(List<double> weights, int budget)
    {
        double total = weights.Sum();
        var raw = weights.Select(w => total > 0 ? budget * w / total : 0.0).ToList();
        var allocations = raw.Select(a => (int)Math.Floor(a)).ToList();
        int remainder = budget - allocations.Sum();

        var order = raw
       .Select((a, i) => (Index: i, Fraction: a - Math.Floor(a)))
   .OrderByDescending(x => x.Fraction)
            .Select(x => x.Index);

        foreach (var idx in order.Take(remainder))
            allocations[idx]++;

        return allocations;
    }

    private static NodeMeta PickBest(
        IEnumerable<NodeMeta> candidates,
        HashSet<string> selected,
      HashSet<string> mastered,
      HashSet<string> covered)
    {
        NodeMeta best = null;
        double bestScore = double.MinValue;

        foreach (var node in candidates)
        {
            if (selected.Contains(node.Node.Id) || mastered.Contains(node.Node.Id)) continue;
            if (!node.Ancestors.IsSubsetOf(mastered)) continue;

            double score = MarginalGain(node, covered);
            if (score > bestScore)
            {
                bestScore = score;
                best = node;
            }
        }

        return best;
    }

    private static void Commit(
        NodeMeta node,
        HashSet<string> selected,
        HashSet<string> covered,
        HashSet<string> mastered)
    {
        selected.Add(node.Node.Id);
        covered.Add(node.Node.Id);
        covered.UnionWith(node.Ancestors);
        covered.UnionWith(node.Descendants);
        mastered.Add(node.Node.Id);
    }

    private static double MarginalGain(NodeMeta node, HashSet<string> covered)
    {
        int novel = covered.Contains(node.Node.Id) ? 0 : 1;
        foreach (var d in node.Descendants) if (!covered.Contains(d)) novel++;
        foreach (var a in node.Ancestors) if (!covered.Contains(a)) novel++;

        return node.StructuralInfluence * (1 + novel);
    }
}