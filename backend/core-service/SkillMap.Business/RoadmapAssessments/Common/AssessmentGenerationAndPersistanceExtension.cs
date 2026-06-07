using System.Linq;
using System.Threading;

using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using MediatR;

using SkillMap.Business.Abstractions;
using SkillMap.Business.RoadmapAssessments.Features.CreateIntermediateAssessment;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.Tasks;

namespace SkillMap.Business.RoadmapAssessments.Common;
internal static class AssessmentGenerationAndPersistanceExtension
{
    private static TopicQuestionsSettingDto CreateQuestionSetting(LeaningItemAssessment learningItem)
    {
        return new TopicQuestionsSettingDto
        {
            DifficultyLevel = Difficulty.Medium,
            QuestionsCount = 1,
            Types = RoadmapAssessmentConstant.SupportedQuestionTypes.ToList(),
        };
    }
    private static (Topic Topic, TopicQuestionsSettingDto) CreateLearningItemQuestionSettings(LeaningItemAssessment learningItem, TopicQuestionsSettingDto topicQuestionsSetting)
        => (new Topic(learningItem.Id, learningItem.Title, learningItem.Description), topicQuestionsSetting);
    public static async Task<List<TopicQuestionsDto>> GenerateAssessmentQuestions(this IQuestionsGenerator questionsGenerator, List<LeaningItemAssessment> item, Dictionary<string, TopicQuestionsSettingDto> questionSettingsByLearningItem, CancellationToken ct)
    {
        var learningItemQuestionSettings = item.Select(i => CreateLearningItemQuestionSettings(i, questionSettingsByLearningItem[i.Id])).ToList();
        return await questionsGenerator.GenerateQuestionsForTopics(learningItemQuestionSettings, ct);
    }

    private static async Task<long> SaveAssessment(this IRepository<RoadmapAssessment> repository,
        long workspaceId, string roadmapId,
        RoadmapAssessmentType assessmentType,
        Dictionary<string, TopicQuestionsSettingDto> questionSettingsByLearningItem,
        List<TopicQuestionsDto> createdLearningItemQuestions,

        CancellationToken ct)
    {
        var assessment = new RoadmapAssessmentDto
        {
            RoadmapId = roadmapId,
            WorkspaceId = workspaceId.ToString(),
            Type = assessmentType.ToString(),
            TopicQuestions = createdLearningItemQuestions,
            TopicSettings = questionSettingsByLearningItem,
            TestConfig = new RoadmapAssessmentConfigDto
            {
                DifficultyLevel = Difficulty.Medium.ToDifficultyString()
            },
        };
        var entity = new RoadmapAssessment
        {
            RoadmapWorkspaceId = workspaceId,
            TestType = assessmentType.ToFriendlyString(),
        };

        await entity.SetRoadmapTest(assessment.ToEntityModel(), ct);
        await repository.AddAsync(entity, ct);
        await repository.SaveChangesAsync(ct);
        return entity.Id;
    }

    public static async Task<long> SaveInitialAssessment(this IRepository<RoadmapAssessment> repository,
        long workspaceId, string roadmapId,
        Dictionary<string, TopicQuestionsSettingDto> questionSettingsByLearningItem,
        List<TopicQuestionsDto> createdLearningItemQuestions,
        CancellationToken ct)
        => await SaveAssessment(repository, workspaceId, roadmapId, RoadmapAssessmentType.Initial, questionSettingsByLearningItem, createdLearningItemQuestions, ct);

    public static async Task<long> SaveIntermediateAssessment(this IRepository<RoadmapAssessment> repository,
        long workspaceId, string roadmapId,
        Dictionary<string, TopicQuestionsSettingDto> questionSettingsByLearningItem,
        List<TopicQuestionsDto> createdLearningItemQuestions,
        CancellationToken ct)
        => await SaveAssessment(repository, workspaceId, roadmapId, RoadmapAssessmentType.Intermediate, questionSettingsByLearningItem, createdLearningItemQuestions, ct);

    private static async Task<long> GenerateAndSaveAssessment(this IQuestionsGenerator questionsGenerator,
        IRepository<RoadmapAssessment> repository,
        long workspaceId, string roadmapId,
        RoadmapAssessmentType assessmentType,
        List<LeaningItemAssessment> learningItemsForAssessment,
        CancellationToken ct)
    {
        var questionSettingsByLearningItem = learningItemsForAssessment.ToDictionary(i => i.Id, CreateQuestionSetting);
        var createdLearningItemQuestions = await questionsGenerator.GenerateAssessmentQuestions(learningItemsForAssessment, questionSettingsByLearningItem, ct);
        return await SaveAssessment(repository, workspaceId, roadmapId, assessmentType, questionSettingsByLearningItem, createdLearningItemQuestions, ct);
    }

    public static async Task<long> GenerateAndSaveInitialAssessment(this IQuestionsGenerator questionsGenerator,
        IRepository<RoadmapAssessment> repository,
        long workspaceId, string roadmapId,
        List<LeaningItemAssessment> learningItemsForAssessment,
        CancellationToken ct)
        => await GenerateAndSaveAssessment(questionsGenerator, repository, workspaceId, roadmapId, RoadmapAssessmentType.Initial, learningItemsForAssessment, ct);

    public static async Task<long> GenerateAndSaveIntermediateAssessment(this IQuestionsGenerator questionsGenerator,
        IRepository<RoadmapAssessment> repository,
        long workspaceId, string roadmapId,
        List<LeaningItemAssessment> learningItemsForAssessment,
        CancellationToken ct)
        => await GenerateAndSaveAssessment(questionsGenerator, repository, workspaceId, roadmapId, RoadmapAssessmentType.Intermediate, learningItemsForAssessment, ct);
}
