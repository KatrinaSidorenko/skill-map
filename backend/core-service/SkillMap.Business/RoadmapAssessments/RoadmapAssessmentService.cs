using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using MediatR;

using SkillMap.Business.PersonalizedRoadmaps.Features.GetPersonalizedRoadmap;
using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicQuestionComposers;
using SkillMap.Business.RoadmapsWorkspace;
using SkillMap.Business.UserTest;
using SkillMap.Core.Constants;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

using SingleAnswerQuestionAnalysisResultDto = SkillMap.Business.RoadmapTest.Models.SingleAnswerQuestionAnalysisResultDto;

namespace SkillMap.Business.RoadmapTest;

// todo: refactor to more simple cognitive model
public class RoadmapAssessmentService(
    ITopicQuestionComposer topicQuestionsComposer,
    IAssessmentAttemptService userRoadmapTestService,
    IRoadmapWorkspaceRepository workspaceRepository,
    IMediator mediator) : IRoadmapAssessmentService
{
    private async Task<Result<(long WorkspaceId, List<Node> Nodes, List<Edge> Edges)>> GetUserRoadmapGraph(long userId, string roadmapId, CancellationToken ct)
    {
        var workspace = await workspaceRepository.GetFirstOrDefaultAsync(
            w => w.AuthorId == userId && w.RoadmapId == roadmapId && w.IsActive && !w.IsInAuthorMode, ct);

        if (workspace == null)
        {
            return ResultType.NotFound<(long, List<Node>, List<Edge>)>($"Active workspace for user {userId} and roadmap {roadmapId} not found.");
        }

        var workspaceDto = await mediator.Send(new GetRoadmapWorkspaceQuery(workspace.Id), ct);

        var nodes = workspaceDto.LearningItems.Select(li => new Node
        {
            Id = li.Id,
            Title = li.Title,
            Description = li.Description,
        }).ToList();

        var edges = workspaceDto.LearningItemsConnections.Select(c => new Edge
        {
            Id = c.Id,
            Source = c.FromId,
            Target = c.ToId,
        }).ToList();

        return Result.Success((workspace.Id, nodes, edges));
    }

    private Result<bool> IsValidRoadmapForTestGeneration((List<Node> Nodes, List<Edge> Edges) roadmap)
    {
        if (roadmap.Nodes.Count <= RoadmapTestConstants.MinAmountOfTopics)
        {
            return ResultType.ValidationError<bool>([$"Roadmap should contain more than {RoadmapTestConstants.MinAmountOfTopics} topics for test generation"]);
        }

        return Result.Success(true);
    }

    // todo: config validation
    // move by simpliest way - always create new test
    // but
    // we have 3 states:
    // 1. no test - create new
    // 2. test in progress - return existing
    // 3. test completed - create new

    public async Task<Result<RoadmapTestResultDto>> CreateInitialRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct)
    {
        const RoadmapTestType testType = RoadmapTestType.Initial;

        var roadmapGraphResult = await GetUserRoadmapGraph(userId, roadmapId, ct);
        if (!roadmapGraphResult.IsSuccessWithData()) { return roadmapGraphResult.Map<RoadmapTestResultDto, (long, List<Node>, List<Edge>)>(); }

        var (workspaceId, nodes, edges) = roadmapGraphResult.Data;
        var roadmapGraph = (Nodes: nodes, Edges: edges);

        var validationResult = IsValidRoadmapForTestGeneration(roadmapGraph);
        if (!validationResult.IsSuccessWithData()) { return validationResult.Map<RoadmapTestResultDto, bool>(); }

        var generatedTestResult = await topicQuestionsComposer.GenerateRoadmapTestQuestions(roadmapGraph.Nodes, roadmapGraph.Edges, config, ct);
        if (!generatedTestResult.IsSuccessWithData()) { return ResultType.FailedToCreate<RoadmapTestResultDto>(generatedTestResult.Message); }

        var generatedTest = generatedTestResult.Data;
        var topicsQuestions = generatedTest.ByTopic.SelectMany(t => t.Value.Questions).ToList();
        var topicSettings = generatedTest.ByTopic.ToDictionary(t => t.Key, t => t.Value.CreationSettings);

        var roadmapTest = new RoadmapTestDao
        {
            RoadmapId = roadmapId,
            TopicQuestions = topicsQuestions,
            TopicSettings = topicSettings,
            TestConfig = config
        };

        var testId = await userRoadmapTestService.SaveUserRoadmapTest(userId, workspaceId, roadmapId, testType, roadmapTest, ct);
        return Result.Success(roadmapTest.ToTestResult(testId));
    }

    public async Task<Result<RoadmapTestResultDto>> CreateIntermediateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct)
    {
        const RoadmapTestType testType = RoadmapTestType.Intermediate;

        var roadmapGraphResult = await GetUserRoadmapGraph(userId, roadmapId, ct);
        if (!roadmapGraphResult.IsSuccessWithData()) { return roadmapGraphResult.Map<RoadmapTestResultDto, (long, List<Node>, List<Edge>)>(); }

        var (workspaceId, nodes, edges) = roadmapGraphResult.Data;
        var roadmapGraph = (Nodes: nodes, Edges: edges);

        var validationResult = IsValidRoadmapForTestGeneration(roadmapGraph);
        if (!validationResult.IsSuccessWithData()) { return validationResult.Map<RoadmapTestResultDto, bool>(); }

        var generatedTestResult = await topicQuestionsComposer.GenerateRoadmapTestQuestions(roadmapGraph.Nodes, roadmapGraph.Edges, config, ct);
        if (!generatedTestResult.IsSuccessWithData()) { return ResultType.FailedToCreate<RoadmapTestResultDto>(generatedTestResult.Message); }

        var generatedTest = generatedTestResult.Data;
        var topicsQuestions = generatedTest.ByTopic.Values.SelectMany(v => v.Questions).ToList();
        var topicSettings = generatedTest.ByTopic.ToDictionary(t => t.Key, t => t.Value.CreationSettings);
        var roadmapTest = new RoadmapTestDao
        {
            RoadmapId = roadmapId,
            TopicQuestions = topicsQuestions,
            TopicSettings = topicSettings,
            TestConfig = config
        };

        var testId = await userRoadmapTestService.SaveUserRoadmapTest(userId, workspaceId, roadmapId, testType, roadmapTest, ct);
        return Result.Success(roadmapTest.ToTestResult(testId));
    }

    public async Task<string> EstimateRoadmapTest(string roadmapTestId, RoadmapTestAnswers userAnswers, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var roadmapTestContent = await userRoadmapTestService.GetRoadmapTest(roadmapTestId, ct);
        var questionsEstimationResults = EstimateQuestionsAnswers(roadmapTestContent, userAnswers);
        var topicsEstimationResults = roadmapTestContent.TopicQuestions
            .ToDictionary(t => t.Id, t => new TopicAnswersAnalysisDto(t.Questions.ToDictionary(q => q.Id, q => questionsEstimationResults.GetOrDefault(q.Id))));
        var testAnalysisResult = new RoadmapTestResultsDto(topicsEstimationResults);

        return await userRoadmapTestService.SaveEndOfTakingRoadmapTestWithAnalysis(roadmapTestId, testAnalysisResult, ct);
    }

    // analysis for each question
    private QuestionAnalysisResultDto EstimateQuestionAnswer(QuestionDto questionDto, QuestionAnswer? userAnswer)
    {
        switch (questionDto.Type)
        {
            case TestQuestionType.SingleChoice:
                {
                    var singleChoiceAnswer = userAnswer as SingleChoiceAnswer;
                    var correctAnswer = questionDto.Answers.FirstOrDefault(a => a.IsCorrect);
                    var isCorrect = correctAnswer != null && singleChoiceAnswer != null && singleChoiceAnswer.SelectedAnswerId == correctAnswer.Id;
                    return new SingleAnswerQuestionAnalysisResultDto
                    {
                        QuestionType = questionDto.Type,
                        TotalPossiblePoints = 1,
                        AchievedPoints = isCorrect ? 1 : 0,
                        SelectedAnswerId = singleChoiceAnswer?.SelectedAnswerId,
                        CorrectAnswerId = correctAnswer?.Id
                    };
                }
            default:
                throw new LearningPlatformException(ErrorCode.INTERNALERROR, $"Unsupported question type {questionDto.Type}");
        }
    }

    public async Task<RoadmapChangesSuggestionsDto> GetRoadmapChangesSuggestions(long userId, string roadmapTestResultId, CancellationToken ct)
    {
        var testAnalysisResult = await userRoadmapTestService.GetRoadmapTestAnalysisResult(roadmapTestResultId, ct);

        var roadmapGraphResult = await GetUserRoadmapGraph(userId, testAnalysisResult.RoadmapId, ct);
        var (_, nodes, edges) = roadmapGraphResult.GetDataOrThrowNotFound();

        var workspace = await workspaceRepository.GetFirstOrDefaultAsync(
            w => w.AuthorId == userId && w.RoadmapId == testAnalysisResult.RoadmapId && w.IsActive && !w.IsInAuthorMode,
            ct);
        var workspaceDto = await mediator.Send(new GetRoadmapWorkspaceQuery(workspace!.Id), ct);

        var pointsByTopics = testAnalysisResult.TopicsAnalysis.ToDictionary(
            ta => ta.Key,
            ta => (
            TotalPossible: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.TotalPossiblePoints),
            Achieved: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.AchievedPoints)));

        var suggestedChanges = await new RoadmapModificationAdvisor().SuggestRoadmapTopicChnages(nodes, edges, pointsByTopics);

        var actualRoadmapNodeStatuses = workspaceDto.LearningItems.ToDictionary(li => li.Id, li => li.Status.ToStatusString());
        var changesWithCurrentStateDiff = suggestedChanges.Where(sc =>
        {
            var actualStatus = actualRoadmapNodeStatuses.GetOrDefault(sc.Id);
            if (actualStatus == null)
                return false;

            var nodeMarkToLearningStatus = sc.MarkType.ToLearningStatusString() ?? actualStatus;
            return actualStatus != nodeMarkToLearningStatus;
        }).ToList();

        return new RoadmapChangesSuggestionsDto
        {
            Suggestions = changesWithCurrentStateDiff.Select(sc => new RoadmapTestSuggestionItemDto
            {
                LearningItemId = sc.Id,
                LearningStatus = sc.MarkType.ToLearningStatusString(),
                Title = sc.Title,
                Description = sc.Description
            }).ToList()
        };
    }

    private Dictionary<string, QuestionAnalysisResultDto> EstimateQuestionsAnswers(RoadmapTestDao roadmapTest, RoadmapTestAnswers userAnswers)
    {
        var userAnswersDict = userAnswers.QuestionAnswers.ToDictionary(qa => qa.QuestionId, qa => qa);
        return roadmapTest.TopicQuestions
            .SelectMany(t => t.Questions)
            .ToDictionary(q => q.Id, q => EstimateQuestionAnswer(q, userAnswersDict.GetOrDefault(q.Id)));
    }
}