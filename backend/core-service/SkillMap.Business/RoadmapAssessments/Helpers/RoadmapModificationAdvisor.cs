using LearningPlatform.Roadmap.Business.Contracts.Models;

using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapTest.Helpers;

public enum NodeMarkType
{
    Finished,
    NeedsReview,
    Uncertain,
    Untested,
}

public class MarkNode : Node
{
    [JsonProperty("markType")]
    public NodeMarkType MarkType { get; set; }
}

public class RoadmapModificationAdvisor
{
    // Thresholds
    private const double PassThreshold = 0.75; // 75%
    private const double FailThreshold = 0.40; // 40%

    public async Task<List<MarkNode>> SuggestRoadmapTopicChnages(
        List<Node> nodes,
        List<Edge> edges,
        Dictionary<string, (int TotalPossible, int Achieved)> testPoints)
    {
        // 1. Initialize the Graph (Parents/Children relationships)
        var childToParents = new Dictionary<string, List<string>>();
        var parentToChildren = new Dictionary<string, List<string>>();

        // Initialize collections
        foreach (var node in nodes)
        {
            childToParents[node.Id] = new List<string>();
            parentToChildren[node.Id] = new List<string>();
        }

        foreach (var edge in edges)
        {
            if (edge.Source != null && edge.Target != null)
            {
                if (childToParents.ContainsKey(edge.Target)) childToParents[edge.Target].Add(edge.Source);
                if (parentToChildren.ContainsKey(edge.Source)) parentToChildren[edge.Source].Add(edge.Target);
            }
        }

        // 2. Convert to MarkNodes and set Initial State
        // We start with a dictionary for quick lookups during processing
        var nodeStates = nodes.ToDictionary(n => n.Id, n => new MarkNode
        {
            Id = n.Id,
            Title = n.Title,
            Description = n.Description,
            MarkType = NodeMarkType.Untested // Default state
        });

        // =========================================================
        // PHASE 1: DIRECT EVALUATION (Traffic Light: Check Scores)
        // =========================================================
        foreach (var (nodeId, scores) in testPoints)
        {
            if (!nodeStates.ContainsKey(nodeId)) continue;

            double score = scores.TotalPossible > 0
                ? (double)scores.Achieved / scores.TotalPossible
                : 0;

            if (score >= PassThreshold)
            {
                nodeStates[nodeId].MarkType = NodeMarkType.Finished;
            }
            else if (score <= FailThreshold)
            {
                nodeStates[nodeId].MarkType = NodeMarkType.NeedsReview;
            }
            else
            {
                // "Yellow" state - barely passed, but not completed. 
                // We leave it as InProgress (needs more study)
                nodeStates[nodeId].MarkType = NodeMarkType.Uncertain;
            }
        }

        // =========================================================
        // PHASE 2: PROPAGATION (Cascading Logic)
        // =========================================================

        // We perform a Topological Sort or iterative pass to propagate states.
        // A simple iterative approach works fine for finding downstream impacts.
        bool changed;
        int maxIterations = nodes.Count; // Safety break

        do
        {
            changed = false;
            maxIterations--;

            foreach (var node in nodes)
            {
                var currentState = nodeStates[node.Id];
                var parents = childToParents[node.Id];

                // RULE A: Cascading Failure (Red Light)
                // If ANY parent needs review, the child definitely needs review (or is blocked).
                if (currentState.MarkType != NodeMarkType.NeedsReview)
                {
                    bool parentFailed = parents.Where(p => nodeStates.ContainsKey(p)).Any(pId => nodeStates[pId].MarkType == NodeMarkType.NeedsReview);
                    if (parentFailed)
                    {
                        currentState.MarkType = NodeMarkType.NeedsReview;
                        changed = true;
                    }
                }

                // RULE B: Auto-Completion (Green Light Propagation - Optional)
                // If you want logic where completing hard stuff auto-completes easy stuff:
                // Implement here (Parents = Completed if Child = Completed). 
                // *Skipped for now as it's risky without strong verification*
            }
        } while (changed && maxIterations > 0);

        // =========================================================
        // PHASE 3: FINALIZATION
        // =========================================================

        // Logic for Untested Nodes:
        // If a node is InProgress but ALL parents are Completed, it remains InProgress (Ready to start).
        // If a node is InProgress but parents are NOT Completed, it is effectively "Locked".
        // Since your Enum doesn't have "Locked", we leave them as "InProgress" or could downgrade them.
        // For this specific Enum, the current state is sufficient.

        return nodeStates.Values.ToList();
    }
}