using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Shared.Extensions;

namespace SkillMap.Business.RoadmapAssessments.Common;
internal static class LearningRoadmapStatusesPropagation
{
    internal static List<LeaningItemAssessment> PropagateLearningItemStatuses(RoadmapSnapshot targetSnapshotContent)
    {
        var snapshotCopy = targetSnapshotContent.DeepCopy();
        var itemsMap = snapshotCopy.LearningItems
            .Select(li => LeaningItemAssessment.FromLearningItemSnapshot(li, null))
            .ToDictionary(li => li.Id);

        var childrenMap = snapshotCopy.LearningItemsConnections
            .GroupBy(c => c.FromId)
            .ToDictionary(g => g.Key, g => g.Select(c => c.ToId).ToList());

        var visited = new HashSet<string>();
        var evaluating = new HashSet<string>();

        void ForceAssumeCompletedDownwards(string id)
        {
            var node = itemsMap[id];
            if (node.Status == LearningStatus.Skip || node.Status == LearningStatus.Completed) return;

            itemsMap[id] = node with { Assumption = AssessmentAssumption.AssumedCompleted };
            var childIds = childrenMap.GetOrDefault(id, []);
            foreach (var childId in childIds)
            {
                if (itemsMap[childId].Type.Equals(LearningItemType.SubTopic, StringComparison.OrdinalIgnoreCase))
                {
                    ForceAssumeCompletedDownwards(childId);
                }
            }
        }

        LeaningItemAssessment EvaluateNode(string nodeId)
        {
            if (visited.Contains(nodeId)) return itemsMap[nodeId];
            if (evaluating.Contains(nodeId)) throw new InvalidOperationException($"Cycle detected at node: {nodeId}");

            evaluating.Add(nodeId);
            var currentNode = itemsMap[nodeId];
            var childIds = childrenMap.GetOrDefault(nodeId, []);
            if (childIds.Count <= 0)
            {
                evaluating.Remove(nodeId);
                visited.Add(nodeId);
                return itemsMap[nodeId];
            }

            foreach (var childId in childIds)
            {
                EvaluateNode(childId);
            }

            var subtopics = childIds.Select(id => itemsMap[id]).Where(c => c.Type.Equals(LearningItemType.SubTopic, StringComparison.OrdinalIgnoreCase)).ToList();
            var dependentTopics = childIds.Select(id => itemsMap[id]).Where(c => c.Type.Equals(LearningItemType.Topic, StringComparison.OrdinalIgnoreCase)).ToList();

            bool overriddenByAdvanced = false;
            if (currentNode.Status != LearningStatus.Skip && dependentTopics.Count > 0)
            {
                var hasAdvancedProgress = dependentTopics.Any(t =>
                    t.Status == LearningStatus.InProgress ||
                    t.Status == LearningStatus.Completed ||
                    t.Assumption == AssessmentAssumption.AssumedCompleted ||
                    t.Assumption == AssessmentAssumption.AssumedInProgress);

                if (hasAdvancedProgress)
                {
                    overriddenByAdvanced = true;

                    if (currentNode.Status != LearningStatus.Completed)
                    {
                        currentNode = currentNode with { Assumption = AssessmentAssumption.AssumedCompleted };
                        itemsMap[nodeId] = currentNode;
                    }

                    foreach (var st in subtopics)
                    {
                        ForceAssumeCompletedDownwards(st.Id);
                    }
                }
            }

            if (!overriddenByAdvanced && subtopics.Count > 0)
            {
                subtopics = childIds.Select(id => itemsMap[id])
                    .Where(c => c.Type.Equals(LearningItemType.SubTopic, StringComparison.OrdinalIgnoreCase)).ToList();

                bool allSkipped = subtopics.All(c => c.Status == LearningStatus.Skip);
                bool allCompleted = subtopics.All(c => c.Status == LearningStatus.Completed || c.Assumption == AssessmentAssumption.AssumedCompleted);
                bool allNotStarted = subtopics.All(c => c.Status == LearningStatus.NotStarted && c.Assumption == null);

                if (allSkipped)
                {
                    currentNode = currentNode with { Status = LearningStatus.Skip, Assumption = null };
                }
                else if (allCompleted)
                {
                    bool isStrictlyExplicitCompleted = subtopics.All(c => c.Status == LearningStatus.Completed && c.Assumption == null);

                    currentNode = currentNode with
                    {
                        Status = isStrictlyExplicitCompleted ? LearningStatus.Completed : currentNode.Status,
                        Assumption = isStrictlyExplicitCompleted ? null : AssessmentAssumption.AssumedCompleted
                    };
                }
                else if (allNotStarted)
                {
                    currentNode = currentNode with { Status = LearningStatus.NotStarted, Assumption = null };
                }
                else
                {
                    currentNode = currentNode with { Assumption = AssessmentAssumption.AssumedInProgress };
                }

                itemsMap[nodeId] = currentNode;
            }

            evaluating.Remove(nodeId);
            visited.Add(nodeId);
            return itemsMap[nodeId];
        }

        foreach (var nodeId in itemsMap.Keys)
        {
            EvaluateNode(nodeId);
        }

        return itemsMap.Values.ToList();
    }

    internal static List<LeaningItemAssessment> GetValidatedAssessmentSuggestions(
        List<LeaningItemAssessment> actualRoadmapWithPropagatedStatuses,
        List<LearningItemSuggestion> suggestions,
        List<LearningItemsConnectionSnapshot> connections)
    {
        var actualDict = actualRoadmapWithPropagatedStatuses.ToDictionary(a => a.Id);
        var suggestionsDict = suggestions.ToDictionary(s => s.Id);
        var parentsMap = connections
            .GroupBy(c => c.ToId)
            .ToDictionary(g => g.Key, g => g.Select(c => c.FromId).ToList());

        var validatedDict = new Dictionary<string, LeaningItemAssessment>();
        var evaluating = new HashSet<string>();

        LeaningItemAssessment EvaluateNode(string nodeId)
        {
            var alreadyValidated = validatedDict.GetOrDefault(nodeId);
            if (alreadyValidated != null) return alreadyValidated;

            if (evaluating.Contains(nodeId)) throw new InvalidOperationException($"Cycle detected at node: {nodeId}");
            evaluating.Add(nodeId);

            var actualNode = actualDict[nodeId];
            var parents = parentsMap.GetOrDefault(nodeId, []);

            var areAllParentsCompleted = true;
            foreach (var parentId in parents)
            {
                if (!actualDict.ContainsKey(parentId)) continue;

                var parentResult = EvaluateNode(parentId);
                var parentStatus = parentResult.GetLearningStatus();

                if (parentStatus != LearningStatus.Completed && parentStatus != LearningStatus.Skip)
                {
                    areAllParentsCompleted = false;
                    break;
                }
            }

            LeaningItemAssessment resultNode;
            var suggestion = suggestionsDict.GetOrDefault(nodeId);

            if (suggestion != null)
            {
                var suggestsCompleted = 
                    suggestion.SuggestedStatus == LearningStatus.Completed || 
                    suggestion.SuggestedStatus == LearningStatus.Upcoming ||
                    suggestion.SuggestedStatus == LearningStatus.InProgress ||
                    suggestion.Assumption == AssessmentAssumption.AssumedCompleted;

                if (suggestsCompleted && !areAllParentsCompleted)
                {
                    resultNode = actualNode with { Assumption = null };
                }
                else
                {
                    resultNode = actualNode with
                    {
                        Status = suggestion.SuggestedStatus,
                        Assumption = suggestion.Assumption
                    };
                }
            }
            else
            {
                if (!areAllParentsCompleted && actualNode.Assumption == AssessmentAssumption.AssumedCompleted)
                {
                    resultNode = actualNode with { Assumption = null };
                }
                else
                {
                    resultNode = actualNode;
                }
            }

            evaluating.Remove(nodeId);
            validatedDict[nodeId] = resultNode;
            return resultNode;
        }

        foreach (var nodeId in actualDict.Keys)
        {
            EvaluateNode(nodeId);
        }

        return validatedDict.Values.ToList();
    }
}