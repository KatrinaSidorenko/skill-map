using JetBrains.Annotations;

using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapAssessments.Features.CreateIntermediateAssessment;

[UsedImplicitly]
internal class CreateIntermediateAssessmentHandler(
    IRoadmapWorkspaceEditor workspaceEditor,
    IQuestionsGenerator questionsGenerator,
    IRepository<RoadmapAssessment> repository)
    : IRequestHandler<CreateIntermediateAssessmentCommand, long>
{
    private readonly Random _rnd = new();

    public async Task<long> Handle(CreateIntermediateAssessmentCommand request, CancellationToken cancellationToken)
    {
        var workspaceSnapshot = await workspaceEditor.GetActualRoadmapWorkspaceSnapshot(request.WorkspaceId, cancellationToken);
        if (!workspaceSnapshot.HasEnoughDataToCreateAssessment()) throw new NoContentException();
        var learningItemsForAssessment = PickLearningItemsForAssessment(workspaceSnapshot, RoadmapAssessmentConstant.DefaultQuestionsAmount, _rnd);
        return await questionsGenerator.GenerateAndSaveIntermediateAssessment(repository, request.WorkspaceId, workspaceSnapshot.Id, learningItemsForAssessment, cancellationToken);
    }
    private List<LeaningItemAssessment> PickLearningItemsForAssessment(RoadmapSnapshot snapshot, int k, Random rnd)
    {
        var assessedItems = LearningRoadmapStatusesPropagation.PropagateLearningItemStatuses(snapshot);
        var assessedConnections = snapshot.LearningItemsConnections
            .Select(LearningItemsConnectionAssessment.FromLearningItemsConnectionSnapshot)
            .ToList();

        var M = assessedItems
            .Where(t => t.Status == LearningStatus.Completed ||
                        t.Assumption == AssessmentAssumption.AssumedCompleted ||
                        t.Status == LearningStatus.Skip)
            .Select(t => t.Id)
            .ToHashSet();

        var graphMeta = BuildGraphForSelection(assessedItems, assessedConnections);

        int maxLevel = graphMeta.Values.Any() ? graphMeta.Values.Max(n => n.Level) : 0;
        foreach (var node in graphMeta.Values)
        {
            node.Impact = maxLevel - node.Level + 1;
        }

        var C = new HashSet<string>();
        var selectedItems = new List<LeaningItemAssessment>();

        while (selectedItems.Count < k)
        {
            var frontier = graphMeta.Values
                .Where(n => !M.Contains(n.Id))
                .Where(n => n.Parents.IsSubsetOf(M))
                .ToList();

            if (frontier.Where(n => string.Equals(n.Item.Type, LearningItemType.SubTopic, StringComparison.OrdinalIgnoreCase)).Count() <= 0)
            {
                var frontierTopics = frontier.Where(n => string.Equals(n.Item.Type, LearningItemType.Topic, StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var topic in frontierTopics)
                {
                    M.Add(topic.Id);
                }
            }

            frontier = graphMeta.Values
                .Where(n => !M.Contains(n.Id))
                .Where(n => n.Parents.IsSubsetOf(M))
                .ToList();
            if (frontier.Count <= 0) break;

            var availableSubTopics = frontier
                .Where(n => string.Equals(n.Item.Type, LearningItemType.SubTopic, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (availableSubTopics.Count <= 0) continue;

            NodeMeta bestNode = null;
            var maxGain = double.MinValue;

            foreach (var node in availableSubTopics)
            {
                int coveredCount = 0;
                if (!C.Contains(node.Id)) coveredCount++;

                foreach (var anccId in node.Ancestors)
                {
                    if (!C.Contains(anccId)) coveredCount++;
                }

                var gain = coveredCount * node.Impact;

                if (gain > maxGain)
                {
                    maxGain = gain;
                    bestNode = node;
                }
            }

            if (bestNode != null)
            {
                selectedItems.Add(bestNode.Item);

                C.Add(bestNode.Id);
                C.UnionWith(bestNode.Ancestors);

                M.Add(bestNode.Id);
            }
            else
            {
                break; 
            }
        }

        if (selectedItems.Count < k)
        {
            var unusedSubtopics = assessedItems
                .Where(x => string.Equals(x.Type, LearningItemType.SubTopic, StringComparison.OrdinalIgnoreCase)
                            && !M.Contains(x.Id)
                            && !selectedItems.Any(s => s.Id == x.Id))
                .OrderBy(x => graphMeta.ContainsKey(x.Id) ? graphMeta[x.Id].Level : 0) // Безпечний доступ
                .Take(k - selectedItems.Count);

            selectedItems.AddRange(unusedSubtopics);
        }

        return selectedItems;
    }

    // todo: extract to util methods
    private class NodeMeta
    {
        public LeaningItemAssessment Item { get; }
        public string Id => Item.Id;
        public int Level { get; set; } = 0;
        public double Impact { get; set; } = 0;
        public HashSet<string> Parents { get; set; } = new();
        public HashSet<string> Ancestors { get; } = new();
        public HashSet<string> Descendants { get; } = new();
        public HashSet<string> Coverage => Ancestors.Union(Descendants).Union(new[] { Id }).ToHashSet();

        public NodeMeta(LeaningItemAssessment item)
        {
            Item = item;
        }
    }

    private Dictionary<string, NodeMeta> BuildGraphForSelection(
        List<LeaningItemAssessment> items,
        List<LearningItemsConnectionAssessment> edges)
    {
        var meta = items.ToDictionary(i => i.Id, i => new NodeMeta(i));

        var forward = edges.GroupBy(e => e.FromId).ToDictionary(g => g.Key, g => g.Select(e => e.ToId).ToList());
        var backward = edges.GroupBy(e => e.ToId).ToDictionary(g => g.Key, g => g.Select(e => e.FromId).ToList());

        foreach (var node in meta.Values)
        {
            if (backward.TryGetValue(node.Id, out var parents))
            {
                node.Parents = parents.ToHashSet();
            }
        }

        var inDegree = items.ToDictionary(i => i.Id, i => 0);
        foreach (var edge in edges)
        {
            if (inDegree.ContainsKey(edge.ToId))
                inDegree[edge.ToId]++;
        }

        var queue = new Queue<string>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            if (forward.TryGetValue(currentId, out var neighbors))
            {
                foreach (var neighborId in neighbors)
                {
                    meta[neighborId].Level = Math.Max(meta[neighborId].Level, meta[currentId].Level + 1);

                    inDegree[neighborId]--;
                    if (inDegree[neighborId] == 0)
                    {
                        queue.Enqueue(neighborId);
                    }
                }
            }
        }

        foreach (var node in meta.Values)
        {
            DFS(node.Id, node.Descendants, forward);
            DFS(node.Id, node.Ancestors, backward);
        }

        return meta;
    }

    private void DFS(string start, HashSet<string> visited, Dictionary<string, List<string>> adjacency)
    {
        if (!adjacency.TryGetValue(start, out var neighbors)) return;

        foreach (var neighbor in neighbors)
        {
            if (visited.Add(neighbor)) DFS(neighbor, visited, adjacency);
        }
    }
}