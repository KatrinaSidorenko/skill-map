//using LearningPlatform.Roadmap.Business.Contracts.Models;
//using LearningPlatform.RoadmapTests.Contracts;
//using LearningPlatform.RoadmapTests.Contracts.Models;

//using MediatR;

//using SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
//using SkillMap.Business.RoadmapTest.Models;
//using SkillMap.Business.RoadmapTest.TopicQuestionComposers;
//using SkillMap.Business.RoadmapsWorkspace;
//using SkillMap.Business.UserTest;
//using SkillMap.Core.Constants;
//using SkillMap.Shared.Extensions;
//using SkillMap.Shared.Results;

//using SingleAnswerQuestionAnalysisResultDto = SkillMap.Business.RoadmapTest.Models.SingleAnswerQuestionAnalysisResultDto;
//using SkillMap.Business.RoadmapAssessments.Common;

//namespace SkillMap.Business.RoadmapTest;

//// todo: refactor to more simple cognitive model
//public class RoadmapAssessmentService(
//    ITopicQuestionComposer topicQuestionsComposer,
//    IAssessmentAttemptService userRoadmapTestService,
//    IRoadmapWorkspaceRepository workspaceRepository,
//    IMediator mediator) : IRoadmapAssessmentService
//{
//    // todo: config validation
//    // move by simpliest way - always create new test
//    // but
//    // we have 3 states:
//    // 1. no test - create new
//    // 2. test in progress - return existing
//    // 3. test completed - create new

//    public async Task<Result<RoadmapTestResultDto>> CreateInitialRoadmapTest(long workspaceId, RoadmapTestConfigDto config, CancellationToken ct)
//    {
//        const RoadmapAssessmentType testType = RoadmapAssessmentType.Initial;

//        var roadmapGraphResult = await GetUserRoadmapGraph(workspaceId, ct);
//        if (!roadmapGraphResult.IsSuccessWithData()) { return roadmapGraphResult.Map<RoadmapTestResultDto, (string, List<Node>, List<Edge>)>(); }

//        var (roadmapId, nodes, edges) = roadmapGraphResult.Data;
//        var roadmapGraph = (Nodes: nodes, Edges: edges);

//        var validationResult = IsValidRoadmapForTestGeneration(roadmapGraph);
//        if (!validationResult.IsSuccessWithData()) { return validationResult.Map<RoadmapTestResultDto, bool>(); }

//        var generatedTestResult = await topicQuestionsComposer.GenerateRoadmapTestQuestions(roadmapGraph.Nodes, roadmapGraph.Edges, config, ct);
//        if (!generatedTestResult.IsSuccessWithData()) { return ResultType.FailedToCreate<RoadmapTestResultDto>(generatedTestResult.Message); }

//        var generatedTest = generatedTestResult.Data;
//        var topicsQuestions = generatedTest.ByTopic.SelectMany(t => t.Value.Questions).ToList();
//        var topicSettings = generatedTest.ByTopic.ToDictionary(t => t.Key, t => t.Value.CreationSettings);

//        var roadmapTest = new RoadmapTestDao
//        {
//            RoadmapId = roadmapId,
//            WorkspaceId = workspaceId.ToString(),
//            TopicQuestions = topicsQuestions,
//            TopicSettings = topicSettings,
//            TestConfig = config,
//            Type = testType.ToString()
//        };

//        var testId = await userRoadmapTestService.SaveUserRoadmapTest(workspaceId, roadmapId, testType, roadmapTest, ct);
//        return Result.Success(roadmapTest.ToTestResult(testId));
//    }

//    public async Task<RoadmapChangesSuggestionsDto> GetRoadmapChangesSuggestions(long userId, string roadmapTestResultId, CancellationToken ct)
//    {
//        var testAnalysisResult = await userRoadmapTestService.GetRoadmapTestAnalysisResult(roadmapTestResultId, ct);
//        var workspaceId = long.Parse(testAnalysisResult.WorkspaceId);
//        var workspace = await workspaceRepository.GetFirstOrDefaultAsync(
//           w => w.AuthorId == userId && w.Id == workspaceId && w.IsActive && !w.IsInAuthorMode,
//           ct);
//        if (workspace == null) {
//            throw new LearningPlatformException(ErrorCode.NOTFOUND, $"Active workspace for user {userId} and roadmap {testAnalysisResult.WorkspaceId} not found.");
//        }

//        var roadmapGraphResult = await GetUserRoadmapGraph(workspace.Id, ct);
//        var (_, nodes, edges) = roadmapGraphResult.GetDataOrThrowNotFound();


//        var workspaceDto = await mediator.Send(new GetRoadmapWorkspaceQuery(workspace!.Id), ct);

//        var pointsByTopics = testAnalysisResult.TopicsAnalysis.ToDictionary(
//            ta => ta.Key,
//            ta => (
//            TotalPossible: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.TotalPossiblePoints),
//            Achieved: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.AchievedPoints)));

//        var suggestedChanges = await new RoadmapModificationAdvisor().SuggestRoadmapTopicChnages(nodes, edges, pointsByTopics);

//        var actualRoadmapNodeStatuses = workspaceDto.LearningItems.ToDictionary(li => li.Id, li => li.Status.ToStatusString());
//        var changesWithCurrentStateDiff = suggestedChanges.Where(sc =>
//        {
//            var actualStatus = actualRoadmapNodeStatuses.GetOrDefault(sc.Id);
//            if (actualStatus == null)
//                return false;

//            var nodeMarkToLearningStatus = sc.MarkType.ToLearningStatusString() ?? actualStatus;
//            return actualStatus != nodeMarkToLearningStatus;
//        }).ToList();

//        return new RoadmapChangesSuggestionsDto
//        {
//            Suggestions = changesWithCurrentStateDiff.Select(sc => new RoadmapTestSuggestionItemDto
//            {
//                LearningItemId = sc.Id,
//                LearningStatus = sc.MarkType.ToLearningStatusString(),
//                Title = sc.Title,
//                Description = sc.Description
//            }).ToList()
//        };
//    }
//}