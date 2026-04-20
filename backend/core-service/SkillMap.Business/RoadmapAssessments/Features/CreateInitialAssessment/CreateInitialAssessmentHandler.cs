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

namespace SkillMap.Business.RoadmapAssessments.Features.CreateInitialAssessment;

[UsedImplicitly]
internal class CreateInitialAssessmentHandler(
    IRoadmapWorkspaceEditor workspaceEditor,
    ITopicQuestionsGenerator questionsGenerator,
    IRepository<RoadmapAssessment> repository)
    : IRequestHandler<CreateInitialAssessmentCommand, long>
{
    private const int DefaultTotalQuestions = 10;
    private const RoadmapAssessmentType TestType = RoadmapAssessmentType.Initial;

    public async Task<long> Handle(CreateInitialAssessmentCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await workspaceEditor.GetActualRoadmapSnapshot(request.WorkspaceId, cancellationToken);
        var selectedTopics = PickLearningItemsForAssessment(snapshot, DefaultTotalQuestions);

        var creationSettings = selectedTopics.ToDictionary(
               t => t.Id,
                    t => new TopicQuestionsSettingDto
                    {
                        DifficultyLevel = Difficulty.Medium,
                        QuestionsCount = 1,
                        Types = RoadmapAssessmentConstant.SupportedQuestionTypes.ToList(),
                    });

        var topicsWithSettings = selectedTopics
            .Select(t => (
                topic: new Topic(t.Id, t.Title, t.Description),
                settings: creationSettings[t.Id]))
            .ToList();

        var generatedTopicQuestions = await questionsGenerator.GenerateTopicsQuestions(topicsWithSettings, cancellationToken);

        var roadmapTest = new CreateIntermediateAssessment.RoadmapAssessmentDto
        {
            RoadmapId = snapshot.Id,
            WorkspaceId = request.WorkspaceId.ToString(),
            Type = TestType.ToString(),
            TopicQuestions = generatedTopicQuestions,
            TopicSettings = creationSettings,
            TestConfig = new CreateIntermediateAssessment.RoadmapAssessmentConfigDto
            {
                DifficultyLevel = Difficulty.Medium.ToDifficultyString()
            },
        };

        var entity = new RoadmapAssessment
        {
            RoadmapWorkspaceId = request.WorkspaceId,
            TestType = RoadmapAssessmentType.Initial.ToFriendlyString(),
        };

        await entity.SetRoadmapTest(roadmapTest.ToEntityModel(), cancellationToken);
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private static readonly double[] DefaultProportions = { 0.50, 0.30, 0.20 };

    private static List<LeaningItemAssessment> PickLearningItemsForAssessment(
        RoadmapSnapshot snapshot,
        int k,
        double[]? proportions = null)
    {
        if (k <= 0) return [];

        var targetProportions = proportions ?? DefaultProportions;

        var assessedItems = LearningRoadmapStatusesPropagation.PropagateLearningItemStatuses(snapshot);
        var assessedConnections = snapshot.LearningItemsConnections
            .Select(LearningItemsConnectionAssessment.FromLearningItemsConnectionSnapshot)
            .ToList();

        var (graph, inDegree) = BuildGraphAndInDegrees(assessedItems, assessedConnections);

        var levels = assessedItems.ToDictionary(i => i.Id, _ => 0);
        var queue = new Queue<string>(inDegree.Where(x => x.Value == 0).Select(x => x.Key));
        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();
            foreach (var neighbor in graph[curr])
            {
                levels[neighbor] = Math.Max(levels[neighbor], levels[curr] + 1);

                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        var subtopicsByLevel = assessedItems
            .Where(i => !string.Equals(i.Type, LearningItemType.Topic, StringComparison.OrdinalIgnoreCase))
            .GroupBy(i => levels[i.Id])
            .OrderBy(g => g.Key)
            .ToList();

        var targets = CalculateLevelTargets(k, subtopicsByLevel.Count, targetProportions);

        var selectedItems = new List<LeaningItemAssessment>();
        var rnd = new Random();
        int carryOver = 0;

        for (int i = 0; i < subtopicsByLevel.Count; i++)
        {
            if (selectedItems.Count >= k) break;

            var pool = subtopicsByLevel[i].OrderBy(_ => rnd.Next()).ToList();

            int currentTarget = targets[i] + carryOver;

            if (i == subtopicsByLevel.Count - 1)
            {
                currentTarget = k - selectedItems.Count;
            }

            int toTake = Math.Min(pool.Count, currentTarget);
            selectedItems.AddRange(pool.Take(toTake));

            carryOver = currentTarget - toTake;
        }

        if (selectedItems.Count < k)
        {
            var usedIds = selectedItems.Select(s => s.Id).ToHashSet();
            var remainingFallback = assessedItems
                .Where(i => !string.Equals(i.Type, LearningItemType.Topic, StringComparison.OrdinalIgnoreCase))
                .Where(i => !usedIds.Contains(i.Id))
                .OrderBy(_ => rnd.Next())
                .Take(k - selectedItems.Count);

            selectedItems.AddRange(remainingFallback);
        }

        return selectedItems;
    }

    private static (Dictionary<string, List<string>> Graph, Dictionary<string, int> InDegree) BuildGraphAndInDegrees(
        List<LeaningItemAssessment> items,
        List<LearningItemsConnectionAssessment> connections)
    {
        var graph = new Dictionary<string, List<string>>();
        var inDegree = new Dictionary<string, int>();

        foreach (var item in items)
        {
            graph[item.Id] = [];
            inDegree[item.Id] = 0;
        }

        foreach (var edge in connections)
        {
            if (graph.ContainsKey(edge.FromId) && graph.ContainsKey(edge.ToId))
            {
                graph[edge.FromId].Add(edge.ToId);
                inDegree[edge.ToId]++;
            }
        }

        return (graph, inDegree);
    }

    private static int[] CalculateLevelTargets(int budget, int levelsCount, double[] proportions)
    {
        var targets = new int[levelsCount];
        int allocated = 0;

        for (int i = 0; i < levelsCount; i++)
        {
            if (i < proportions.Length)
            {
                targets[i] = (int)Math.Round(budget * proportions[i]);
                allocated += targets[i];
            }
        }

        if (targets.Length > 0 && allocated != budget)
        {
            int difference = budget - allocated;
            targets[0] = Math.Max(0, targets[0] + difference);
        }

        return targets;
    }
}
