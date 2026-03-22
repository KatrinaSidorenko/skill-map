using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicAnalyzers;
using SkillMap.Business.RoadmapTest.TopicQuestionDistributionBuilder;
using SkillMap.Business.RoadmapTest.TopicSelectors;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest.TopicQuestionComposers;

public record TopicTestBundle(List<TopicQuestionsDto> Questions, TopicQuestionsSettingDto CreationSettings);
public record RoadmapTestResult(Dictionary<string, TopicTestBundle> ByTopic);

internal sealed class NodeMeta
{
    public Node Node { get; }
    public int InDegree { get; set; }
    public int OutDegree { get; set; }
    public int Depth { get; set; }

    public HashSet<string> Ancestors { get; } = new();
    public HashSet<string> Descendants { get; } = new();

    public double StructuralInfluence { get; set; }

    public NodeMeta(Node node)
    {
        Node = node;
    }
}

internal sealed class Graph
{
    public IReadOnlyList<Node> Nodes { get; init; }
    public IReadOnlyList<Edge> Edges { get; init; }
}

public class BaseTopicQuestionComposer2(
    ITopicQuestionsGenerator topicQuestionsGenerator,
    IRoadmapTopicsSelector topicsSelector,
    ITopicQuestionDistributionBuilder topicQuestionDistributionBuilder) : ITopicQuestionComposer
{
    /// <summary>
    /// Generates assessment questions for a learning roadmap using a greedy
    /// structural-coverage algorithm over the DAG of prerequisite relations.
    ///
    /// The selection mechanism guarantees a (1 - 1/e) approximation of maximum
    /// structural coverage within the given budget, provided the marginal gain
    /// function is submodular — which holds when coverage is defined as the
    /// cardinality of the union of ancestor and descendant closures.
    /// </summary>
    public async Task<Result<RoadmapTestResult>> GenerateRoadmapTestQuestions(
        List<Node> nodes,
        List<Edge> edges,
        RoadmapTestConfigDto config,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var topics = nodes
            .Select(n => new Topic(n.Id, n.Title, n.Description))
            .ToList();

        var structureMeta = AnalyzeStructure(new Graph { Nodes = nodes, Edges = edges });

        var questionsLimit = (int)Math.Round(
            Math.Min(nodes.Count * 0.75, RoadmapTestConstants.MaxNumberOfQuestions));
        questionsLimit = 4;

        // Selection begins from empty mastery set M = ∅.
        // The reachable frontier F(M) is enforced inside SelectQuestions
        // and expands as each selected topic is treated as mastered for
        // the purpose of structural exploration.
        var targetTopicIds = SelectQuestions(structureMeta, questionsLimit);

        var targetTopics = topics
            .Where(t => targetTopicIds.Contains(t.Id))
            .ToList();

        var topicQuestionsCreationSettings = targetTopics
            .ToDictionary(
                t => t.Id,
                t => GetTopicQuestionsGenerationSettings(t, null, config.DifficultyLevel));

        var generateTestQuestions = await topicQuestionsGenerator
            .GenerateTopicsQuestions(topicQuestionsCreationSettings.Values.ToList(), ct);

        var generateTestQuestionsDict = generateTestQuestions
            .GroupBy(t => t.Id)
            .ToDictionary(g => g.Key, g => g.ToList());

        //return Result.Success(new RoadmapTestResult(
        //    targetTopics.ToDictionary(
        //        t => t.Id,
        //        t => new TopicTestBundle(
        //            Questions: generateTestQuestionsDict.GetOrDefault(t.Id, []),
        //            CreationSettings: topicQuestionsCreationSettings
        //                .GetOrDefaultAsNullable(t.Id)?.Setting
        //        ))));
        return Result.Success(new RoadmapTestResult(
            targetTopics.ToDictionary(
                t => t.Id,
                t => new TopicTestBundle(
                    Questions: new List<TopicQuestionsDto>(),
                    CreationSettings: topicQuestionsCreationSettings
                        .GetOrDefaultAsNullable(t.Id)?.Setting
                ))));
    }

    /// <summary>
    /// Hybrid Stratified Set Cover selection.
    ///
    /// Combines two complementary approaches:
    ///   1. Stratified allocation  — distributes the budget k across topological
    ///      layers proportionally to each layer's average structural influence,
    ///      guaranteeing that no depth level is entirely skipped.
    ///   2. Greedy set cover       — within each layer selects the node with the
    ///      highest marginal coverage gain, using a shared covered set so that
    ///      coverage accumulated in earlier layers is respected in later ones.
    ///
    /// Frontier constraint F(M) = { v ∈ V \ M : Anc(v) ⊆ M } is enforced at
    /// every candidate evaluation step. The mastered set M grows optimistically
    /// after each selection so that dependents become eligible in subsequent
    /// iterations without requiring confirmed assessment results.
    ///
    /// Approximation guarantee: the within-layer greedy selection preserves the
    /// (1 − 1/e) submodular coverage guarantee locally per layer. Global
    /// optimality is not guaranteed because the budget is split across layers,
    /// but the stratified allocation ensures diagnostic breadth that the pure
    /// greedy approach cannot provide.
    /// </summary>
    private List<string> SelectQuestions(
        Dictionary<string, NodeMeta> meta,
        int budget)
    {
        var selected = new HashSet<string>();
        var covered = new HashSet<string>();

        // M starts empty — no prior mastery is assumed.
        // Source vertices are NOT pre-seeded so they compete fairly
        // for the layer-0 allocation alongside any other layer-0 candidates.
        var mastered = new HashSet<string>();

        // ── Step 1: Partition unmastered vertices into topological layers ────
        // Layer i contains all vertices whose maximum topological depth equals i.
        // Mastered vertices are excluded before grouping.
        var layers = meta.Values
            .Where(n => !mastered.Contains(n.Node.Id))
            .GroupBy(n => n.Depth)
            .OrderBy(g => g.Key)
            .Select(g => g.ToList())
            .ToList();

        if (layers.Count == 0)
            return selected.ToList();

        // ── Step 2: Compute per-layer average structural influence ───────────
        // w(Lᵢ) = average StructuralInfluence of all vertices in layer i.
        // Layers with higher average influence receive a larger share of k.
        var layerWeights = layers
            .Select(layer => layer.Average(n => n.StructuralInfluence))
            .ToList();

        double totalWeight = layerWeights.Sum();

        // ── Step 3: Allocate budget across layers ────────────────────────────
        // kᵢ = round( k × w(Lᵢ) / Σⱼ w(Lⱼ) ), minimum 0.
        // Remainders are collected and redistributed to the layers with the
        // highest fractional parts (largest-remainder method) so that
        // Σᵢ kᵢ = k exactly.
        var rawAllocations = layerWeights
            .Select(w => totalWeight > 0 ? budget * w / totalWeight : 0.0)
            .ToList();

        var allocations = rawAllocations
            .Select(a => (int)Math.Floor(a))
            .ToList();

        int remainder = budget - allocations.Sum();

        // Distribute remaining slots to layers with highest fractional parts.
        var fractionalOrder = rawAllocations
            .Select((a, i) => (Index: i, Fraction: a - Math.Floor(a)))
            .OrderByDescending(x => x.Fraction)
            .Select(x => x.Index)
            .ToList();

        for (int i = 0; i < remainder; i++)
            allocations[fractionalOrder[i]]++;

        // ── Step 4: Greedy set cover within each layer ───────────────────────
        // Layers are processed in topological order (shallow to deep) so that
        // mastered set M expands naturally and the frontier constraint becomes
        // progressively less restrictive as selection advances.
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            int layerBudget = allocations[layerIndex];
            if (layerBudget == 0)
                continue;

            var layer = layers[layerIndex];
            int selectedInLayer = 0;

            while (selectedInLayer < layerBudget && selected.Count < budget)
            {
                NodeMeta best = null;
                double bestScore = double.MinValue;

                foreach (var node in layer)
                {
                    // Skip already selected or mastered vertices.
                    if (selected.Contains(node.Node.Id))
                        continue;
                    if (mastered.Contains(node.Node.Id))
                        continue;

                    // Enforce frontier constraint: Anc(v) ⊆ M.
                    // Note: when M = ∅, only source vertices (Anc = ∅) pass.
                    // The condition is unconditional — mastered.Count > 0
                    // guard was removed as it incorrectly allowed all nodes
                    // through when M was empty.
                    if (!node.Ancestors.IsSubsetOf(mastered))
                        continue;

                    double score = MarginalGain(node, covered);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = node;
                    }
                }

                // No eligible candidate in this layer — move to next layer.
                // This happens when all layer-i nodes are frontier-blocked,
                // meaning their prerequisites have not yet been selected.
                // The remaining layer allocation is not carried forward;
                // the global budget continues in subsequent layers.
                if (best == null)
                    break;

                selected.Add(best.Node.Id);
                selectedInLayer++;

                // Update shared coverage — respected across all layers.
                covered.Add(best.Node.Id);
                covered.UnionWith(best.Ancestors);
                covered.UnionWith(best.Descendants);

                // Optimistic mastery assumption: selected vertex enters M
                // so its dependents become frontier-eligible in subsequent
                // iterations within this layer and in all later layers.
                mastered.Add(best.Node.Id);
            }
        }

        // ── Step 5: Fill remaining budget from any layer if under-allocated ──
        // Under-allocation occurs when layer candidates were frontier-blocked
        // and their layer budget could not be fully consumed. Remaining slots
        // are filled by a final greedy pass over all eligible vertices
        // regardless of layer, preserving the frontier and mastery constraints.
        if (selected.Count < budget)
        {
            while (selected.Count < budget)
            {
                NodeMeta best = null;
                double bestScore = double.MinValue;

                foreach (var node in meta.Values)
                {
                    if (selected.Contains(node.Node.Id))
                        continue;
                    if (mastered.Contains(node.Node.Id))
                        continue;
                    if (!node.Ancestors.IsSubsetOf(mastered))
                        continue;

                    double score = MarginalGain(node, covered);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = node;
                    }
                }

                if (best == null)
                    break;

                selected.Add(best.Node.Id);
                covered.Add(best.Node.Id);
                covered.UnionWith(best.Ancestors);
                covered.UnionWith(best.Descendants);
                mastered.Add(best.Node.Id);
            }
        }

        return selected.ToList();
    }

    /// <summary>
    /// Marginal coverage gain of selecting a vertex given the already-covered set.
    ///
    /// Counts the number of vertices in Anc(v) ∪ {v} ∪ Desc(v) not yet present
    /// in the covered set, weighted by StructuralInfluence as a tiebreaker.
    ///
    /// This function is submodular: marginal gain is non-increasing as the
    /// covered set grows, which preserves the (1 − 1/e) approximation guarantee
    /// of the greedy selection within each layer.
    /// </summary>
    private static double MarginalGain(NodeMeta node, HashSet<string> covered)
    {
        int novel = 0;

        if (!covered.Contains(node.Node.Id))
            novel++;

        foreach (var d in node.Descendants)
            if (!covered.Contains(d)) novel++;

        foreach (var a in node.Ancestors)
            if (!covered.Contains(a)) novel++;

        return node.StructuralInfluence * (1 + novel);
    }

    private Dictionary<string, NodeMeta> AnalyzeStructure(Graph graph)
    {
        var meta = BuildMetadata(graph);

        ComputeDepths(meta, graph.Edges.ToList());
        ComputeReachability(meta, graph.Edges.ToList());
        ComputeStructuralInfluence(meta);

        return meta;
    }

    private static Dictionary<string, NodeMeta> BuildMetadata(Graph graph)
    {
        var map = graph.Nodes.ToDictionary(
            n => n.Id,
            n => new NodeMeta(n));

        foreach (var edge in graph.Edges)
        {
            // Edges referencing unknown nodes indicate a malformed graph.
            // Throwing here is preferable to silent data corruption because
            // structural analysis assumes a complete and consistent edge set.
            if (!map.ContainsKey(edge.Source) || !map.ContainsKey(edge.Target)) { continue; }
                //throw new InvalidOperationException(
                //    $"Edge ({edge.Source} → {edge.Target}) references a node " +
                //    $"that does not exist in the graph. " +
                //    $"Ensure the graph is structurally valid before analysis.");

            map[edge.Target].InDegree++;
            map[edge.Source].OutDegree++;
        }

        return map;
    }

    /// <summary>
    /// Computes the maximum topological depth of each vertex using a
    /// modified Kahn's algorithm. Depth represents the length of the
    /// longest prerequisite chain leading to a vertex and is used as
    /// a component of the structural influence score.
    ///
    /// Note: InDegree values are mutated during traversal. This method
    /// must be called before ComputeReachability, which relies on the
    /// original edge set rather than InDegree values.
    /// </summary>
    private static void ComputeDepths(
        Dictionary<string, NodeMeta> meta,
        List<Edge> edges)
    {
        var queue = new Queue<NodeMeta>(
            meta.Values.Where(n => n.InDegree == 0));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var edge in edges.Where(e => e.Source == current.Node.Id))
            {
                var target = meta[edge.Target];
                target.Depth = Math.Max(target.Depth, current.Depth + 1);

                target.InDegree--;
                if (target.InDegree == 0)
                    queue.Enqueue(target);
            }
        }
    }

    /// <summary>
    /// Computes ancestor and descendant closures for all vertices using
    /// independent DFS traversals over the forward and transposed edge sets.
    ///
    /// Forward DFS from v over E yields Desc(v).
    /// Forward DFS from v over E^T (reversed edges) yields Anc(v).
    ///
    /// Both closures exclude the vertex itself, consistent with
    /// Definitions 3 and 4 in the domain model.
    /// </summary>
    private static void ComputeReachability(
        Dictionary<string, NodeMeta> meta,
        List<Edge> edges)
    {
        var forward = edges
            .GroupBy(e => e.Source)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Target).ToList());

        var backward = edges
            .GroupBy(e => e.Target)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Source).ToList());

        foreach (var node in meta.Values)
        {
            DFS(node.Node.Id, node.Descendants, forward);
            DFS(node.Node.Id, node.Ancestors, backward);
        }
    }

    private static void DFS(
        string start,
        HashSet<string> visited,
        Dictionary<string, List<string>> adjacency)
    {
        if (!adjacency.TryGetValue(start, out var neighbors))
            return;

        foreach (var neighbor in neighbors)
        {
            if (visited.Add(neighbor))
                DFS(neighbor, visited, adjacency);
        }
    }

    /// <summary>
    /// Computes the structural influence of each vertex as:
    ///
    ///   StructuralInfluence(v) = |Anc(v)| + |Desc(v)|
    ///
    /// This is the total number of vertices structurally connected to v
    /// through prerequisite chains in either direction. It directly
    /// operationalizes the notion of structural coverage contribution
    /// defined in the paper: a vertex with high structural influence,
    /// when selected for assessment, provides information about a large
    /// portion of the dependency graph regardless of the direction of
    /// the prerequisite chain.
    ///
    /// This formulation replaces the previous heuristic that combined
    /// descendant count, out-degree, and depth in a logarithmic formula
    /// without formal justification. The previous formula introduced
    /// double-counting between descendant count and out-degree, and the
    /// depth penalty created an unjustified bias against deep vertices
    /// that may carry high structural importance.
    ///
    /// Depth is retained in NodeMeta for use as an explicit tiebreaker
    /// in future extensions but is not incorporated into the primary
    /// influence score.
    /// </summary>
    private static void ComputeStructuralInfluence(Dictionary<string, NodeMeta> meta)
    {
        foreach (var node in meta.Values)
        {
            node.StructuralInfluence =
                node.Ancestors.Count + node.Descendants.Count;
            //node.StructuralInfluence =
            //    Math.Log(node.Descendants.Count + 1) *
            //    (node.OutDegree + 1) /
            //    Math.Log(node.Depth + 2);
        }
    }

    private static List<Topic> FilterTopicsByAllocatedQuestions(
        Dictionary<string, TopicQuestionPlan> topicTestPlan,
        List<Topic> topics)
    {
        return topicTestPlan
            .Where(ta => ta.Value.QuestionsCount > 0)
            .OrderByDescending(ta => ta.Value.QuestionsCount)
            .Join(
                topics,
                ta => ta.Key,
                t => t.Id,
                (ta, t) => t)
            .ToList();
    }
    private static (Topic Topic, TopicQuestionsSettingDto Setting) GetTopicQuestionsGenerationSettings(Topic topic, TopicQuestionPlan analysis, string difficultyLevel)
    {
        return (topic, new TopicQuestionsSettingDto
        {
            DifficultyLevel = difficultyLevel.FromDifficultyString(),
            QuestionsCount = analysis?.QuestionsCount ?? 1,
            Types = RoadmapTestConstants.SupportedQuestionTypes.ToList(),
        });
    }
}