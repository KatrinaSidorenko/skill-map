using JetBrains.Annotations;

using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.RoadmapTest;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserTest;
using SkillMap.Core.Constants;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Core.Tasks;

namespace SkillMap.Business.RoadmapAssessments.Features.CreateIntermediateAssessment;

[UsedImplicitly]
internal class CreateIntermediateAssessmentHandler(
    IRoadmapWorkspaceEditor workspaceEditor,
    ITopicQuestionsGenerator questionsGenerator,
    IRepository<RoadmapAssessment> repository)
    : IRequestHandler<CreateIntermediateAssessmentCommand, long>
{
    private const int DefaultTotalQuestions = 10;
    private const RoadmapAssessmentType TestType = RoadmapAssessmentType.Intermediate;
    private readonly Random _rnd = new();

    public async Task<long> Handle(CreateIntermediateAssessmentCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await workspaceEditor.GetActualRoadmapSnapshot(request.WorkspaceId, cancellationToken);
        var selectedSubtopics = PickLearningItemsForAssessment(snapshot);
        var creationSettings = selectedSubtopics.ToDictionary(
            s => s.Id,
            s => new TopicQuestionsSettingDto
            {
                DifficultyLevel = Difficulty.Medium,
                QuestionsCount = 1,
                Types = RoadmapTestConstants.SupportedQuestionTypes.ToList(),
            });

        var topicsWithSettings = selectedSubtopics
            .Select(s => (
                topic: new Topic(s.Id, s.Title, s.Description),
                settings: creationSettings[s.Id]))
            .ToList();

        var generatedTopicQuestions = await questionsGenerator.GenerateTopicsQuestions(topicsWithSettings, cancellationToken);

        var roadmapTest = new RoadmapTestDao
        {
            RoadmapId = snapshot.Id,
            WorkspaceId = request.WorkspaceId.ToString(),
            Type = TestType.ToString(),
            TopicQuestions = generatedTopicQuestions,
            TopicSettings = creationSettings,
            TestConfig = new RoadmapTestConfigDto { DifficultyLevel = Difficulty.Medium.ToDifficultyString() },
        };

        var entity = new RoadmapAssessment
        {
            RoadmapWorkspaceId = request.WorkspaceId,
            TestType = RoadmapAssessmentType.Intermediate.ToFriendlyString(),
        };
        await entity.SetRoadmapTest(roadmapTest.ToEntityModel(), cancellationToken);
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
    private List<LeaningItemAssessment> PickLearningItemsForAssessment(RoadmapSnapshot snapshot)
    {
        var assessedItems = LearningRoadmapStatusesPropagation.PropagateLearningItemStatuses(snapshot);

        var assessedConnections = snapshot.LearningItemsConnections
            .Select(LearningItemsConnectionAssessment.FromLearningItemsConnectionSnapshot)
            .ToList();

        var topicIds = assessedItems
            .Where(x => x.Type.Equals(LearningItemType.Topic, StringComparison.OrdinalIgnoreCase))
            .Select(t => t.Id)
            .ToHashSet();

        var topics = assessedItems.Where(x => topicIds.Contains(x.Id)).ToList();
        var subtopics = assessedItems.Where(x => !topicIds.Contains(x.Id)).ToList();

        var topicDependencies = assessedConnections
            .Where(c => topicIds.Contains(c.FromId) && topicIds.Contains(c.ToId))
            .ToList();

        var subtopicToTopicMap = assessedConnections
            .Where(c => topicIds.Contains(c.FromId) && !topicIds.Contains(c.ToId))
            .GroupBy(c => c.ToId)
            .ToDictionary(c => c.Key, c => c.Select(c => c.FromId).ToList());

        var pools = QuestionDistributionCalculator.BuildPools(subtopics);
        var quota = QuestionDistributionCalculator.CalculateQuotas(pools, DefaultTotalQuestions);

        var selected = SimpleSubtopicSelector.Select(pools, quota, _rnd);

        var frontierSelected = FrontierSubtopicSelector.Select(
            frontierPool: pools.Frontier,
            topics: topics,
            topicDependencies: topicDependencies,
            subtopicToTopicMap: subtopicToTopicMap,
            budget: quota.TakeFrontier,
            rnd: _rnd);

        selected.AddRange(frontierSelected);
        return selected;
    }
}