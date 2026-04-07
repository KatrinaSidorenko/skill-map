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

        LeaningItemAssessment EvaluateNode(string nodeId)
        {
            if (visited.Contains(nodeId)) return itemsMap[nodeId];
            if (evaluating.Contains(nodeId)) throw new InvalidOperationException($"Cycle detected at node: {nodeId}");

            evaluating.Add(nodeId);
            var currentNode = itemsMap[nodeId];

            if (childrenMap.TryGetValue(nodeId, out var childIds) && childIds.Any())
            {
                // Сначала рекурсивно вычисляем всех "детей" (DFS)
                var evaluatedChildren = childIds.Select(EvaluateNode).ToList();

                // Разделяем детей на Подтемы (Состав) и Темы (Зависимости)
                var subtopics = evaluatedChildren.Where(c => c.Type.ToLower() == LearningItemType.SubTopic).ToList();
                var dependentTopics = evaluatedChildren.Where(c => c.Type.ToLower() == LearningItemType.Topic).ToList();

                // === ЛОГИКА 1: Композиция (Topic -> Subtopics) ===
                // "if add A-> C so only if B and C are completed A should be completd in another AssumedInprogress"
                if (subtopics.Any())
                {
                    bool allCompleted = subtopics.All(c => c.Status == LearningStatus.Completed || c.Assumption == AssessmentAssumption.AssumedCompleted);
                    bool allNotStarted = subtopics.All(c => c.Status == LearningStatus.NotStarted && c.Assumption == null);
                    bool anyInProgressOrCompleted = subtopics.Any(c =>
                        c.Status == LearningStatus.InProgress ||
                        c.Status == LearningStatus.Completed ||
                        c.Assumption != null);

                    if (allCompleted)
                    {
                        currentNode = currentNode with
                        {
                           // Status = LearningStatus.Completed,
                            Assumption = currentNode.Status == LearningStatus.Completed ? currentNode.Assumption : AssessmentAssumption.AssumedCompleted
                        };
                    }
                    else if (anyInProgressOrCompleted)
                    {
                        currentNode = currentNode with
                        {
                            //Status = LearningStatus.InProgress,
                            Assumption = AssessmentAssumption.AssumedInProgress
                        };
                    }
                    else if (allNotStarted)
                    {
                        currentNode = currentNode with { Status = LearningStatus.NotStarted, Assumption = null };
                    }
                }

                // === ЛОГИКА 2: Зависимости / Каскад вверх (Topic -> Topic) ===
                // "if A and B are topics if B is completed it means that A also should be Completed, if B is in progress A should be completd"
                if (dependentTopics.Any())
                {
                    bool hasAdvancedProgress = dependentTopics.Any(t =>
                        t.Status == LearningStatus.InProgress ||
                        t.Status == LearningStatus.Completed);

                    // Если студент начал или закончил продвинутую тему, база (A) автоматически считается полностью пройденной
                    if (hasAdvancedProgress)
                    {
                        // Важно: переопределяем статус от композиции, так как наличие зависимого прогресса - это 100% пруф знания базы
                        currentNode = currentNode with
                        {
                            // Status = LearningStatus.Completed,
                            Assumption = AssessmentAssumption.AssumedCompleted
                        };

                        // all dependent topics should be also treated as Asssuemd Completed
                        subtopics.ForEach(st =>
                        {
                            if (st.Status != LearningStatus.Completed)
                            {
                                var updatedSubtopic = st with 
                                { 
                                    //Status = LearningStatus.Completed, 
                                    Assumption = AssessmentAssumption.AssumedCompleted 
                                };
                                itemsMap[st.Id] = updatedSubtopic;
                            }
                        });
                    }
                }

                itemsMap[nodeId] = currentNode;
            }

            evaluating.Remove(nodeId);
            visited.Add(nodeId);

            return currentNode;
        }

        foreach (var nodeId in itemsMap.Keys)
        {
            EvaluateNode(nodeId);
        }

        return itemsMap.Values.ToList();
    }
}