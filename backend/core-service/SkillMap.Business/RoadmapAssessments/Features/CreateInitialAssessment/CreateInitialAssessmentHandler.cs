using JetBrains.Annotations;

using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Common;
using SkillMap.Business.RoadmapAssessments.Common.SelectLearningItems;
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
        var selectedTopics = PickLearningItemsForAssessment(snapshot);

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

    private static List<LeaningItemAssessment> PickLearningItemsForAssessment(RoadmapSnapshot snapshot)
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
            .ToDictionary(g => g.Key, g => g.Select(c => c.FromId).ToList());

        return StratifiedRoadmapTopicsSelector.SelectCoreSubtopics(
            subtopicPool: subtopics,
            topics: topics,
            topicDependencies: topicDependencies,
            subtopicToTopicMap: subtopicToTopicMap,
            budget: DefaultTotalQuestions,
            rnd: new Random());
    }
}
