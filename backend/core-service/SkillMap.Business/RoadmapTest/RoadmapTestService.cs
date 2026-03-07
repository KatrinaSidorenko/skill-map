using LearningPlatform.Roadmap.Business;
using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;

using SkillMap.Business.__old.ModifiedRoadmaps;
using SkillMap.Business.__old.ModifiedRoadmaps.Models;
using SkillMap.Business.__old.UserRoadmaps;
using SkillMap.Business.__old.UserRoadmaps.Models;
using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicAnalyzers;
using SkillMap.Business.RoadmapTest.TopicQuestionComposers;
using SkillMap.Business.UserTest;
using SkillMap.Core.Constants;
using SkillMap.Core.Entities.UserRoadmapTest;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

using SingleAnswerQuestionAnalysisResultDto = SkillMap.Business.RoadmapTest.Models.SingleAnswerQuestionAnalysisResultDto;

namespace SkillMap.Business.RoadmapTest;

// todo: refcator to more simple cognitive model
public class RoadmapTestService(
    IUserRoadmapsService userRoadmapsService,
    ITopicQuestionComposer topicQuestionsComposer,
    ICustomizedRoadmapsService customizedRoadmapsService,
    IUserRoadmapTestService userRoadmapTestService) : IRoadmapTestService
{
    private async Task<Result<(List<Node> Nodes, List<Edge> Edges)>> GetUserRoadmapGraph(long userId, string roadmapId, CancellationToken ct)
    {
        var userModifiedRoadmapResult = await customizedRoadmapsService.GetUserModifiedRoadmap(userId, roadmapId, ct);
        if (!userModifiedRoadmapResult.IsSuccessWithData()) { return userModifiedRoadmapResult.Map<(List<Node>, List<Edge>), SavedUerRoadmap>(); }
        var userModifiedRoadmap = userModifiedRoadmapResult.Data;
        var nodes = userModifiedRoadmap.Nodes.Select(n => new Node
        {
            Id = n.Id,
            Title = n.Title,
            Description = n.Description,
        }).ToList();
        return Result.Success((nodes, userModifiedRoadmap.Edges));
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
        var userRoadmapResult = await userRoadmapsService.GetUserRoadmap(userId, roadmapId, ct, isActive: true);
        if (!userRoadmapResult.IsSuccessWithData()) { return userRoadmapResult.Map<RoadmapTestResultDto, UserRoadmapDto>(); }

        var roadmapGraphResult = await GetUserRoadmapGraph(userId, roadmapId, ct);
        if (!roadmapGraphResult.IsSuccessWithData()) { return roadmapGraphResult.Map<RoadmapTestResultDto, (List<Node>, List<Edge>)>(); }

        var roadmapGraph = roadmapGraphResult.Data;
        var validationResult = IsValidRoadmapForTestGeneration(roadmapGraph);
        if (!validationResult.IsSuccessWithData()) { return validationResult.Map<RoadmapTestResultDto, bool>(); }

        var generatedTestResult = await topicQuestionsComposer.GenerateRoadmapTestQuestions(roadmapGraph.Nodes, roadmapGraph.Edges, config, ct);
        if (!generatedTestResult.IsSuccessWithData()) { return ResultType.FailedToCreate<RoadmapTestResultDto>(generatedTestResult.Message); }

        var userRoadmapId = userRoadmapResult.Data.Id;
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

        var testId = await userRoadmapTestService.SaveUserRoadmapTest(userId, userRoadmapId, roadmapId, testType, roadmapTest, ct);
        return Result.Success(roadmapTest.ToTestResult(testId));
    }
    public async Task<Result<RoadmapTestResultDto>> CreateIntermediateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct)
    {
        var testType = RoadmapTestType.Intermediate;
        var userRoadmapResult = await userRoadmapsService.GetUserRoadmap(userId, roadmapId, ct, isActive: true);
        if (!userRoadmapResult.IsSuccessWithData()) { return userRoadmapResult.Map<RoadmapTestResultDto, UserRoadmapDto>(); }

        var userRoadmapId = userRoadmapResult.Data.Id;
        var roadmapGraphResult = await GetUserRoadmapGraph(userId, roadmapId, ct);
        if (!roadmapGraphResult.IsSuccessWithData()) { return roadmapGraphResult.Map<RoadmapTestResultDto, (List<Node>, List<Edge>)>(); }

        var roadmapGraph = roadmapGraphResult.Data;
        var validationResult = IsValidRoadmapForTestGeneration(roadmapGraph);
        if (!validationResult.IsSuccessWithData()) { return validationResult.Map<RoadmapTestResultDto, bool>(); }

        //var targetNodes = roadmapGraph.Nodes.Where(n => n.Status == LearningStatus.InProgress.ToStatusString() || n.Status == LearningStatus.Completed.ToStatusString()).ToList();
        var generatedTestResult = await topicQuestionsComposer.GenerateRoadmapTestQuestions(roadmapGraph.Nodes, roadmapGraph.Edges, config, ct);
        if (!generatedTestResult.IsSuccessWithData()) { return ResultType.FailedToCreate<RoadmapTestResultDto>(generatedTestResult.Message); }

        //var generatedTestResult = await GenerateQuestionsForRoadmapTestByTopics(userId, roadmapId, 
        //    config, ct, 
        //    nodesFilter: n => n.Status == LearningStatus.InProgress.ToStatusString() || n.Status == LearningStatus.Completed.ToStatusString());
        //if (!generatedTestResult.IsSuccessWithData()) { return ResultType.FailedToCreate<RoadmapTestResultDto>(generatedTestResult.Message); }

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
        var testId = await userRoadmapTestService.SaveUserRoadmapTest(userId, userRoadmapId, roadmapId, testType, roadmapTest, ct);
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
        var userModifiedRoadmapResult = await customizedRoadmapsService.GetUserModifiedRoadmap(userId, testAnalysisResult.RoadmapId, ct);
        var userModifiedRoadmap = userModifiedRoadmapResult.GetDataOrThrowNotFound();
        var pointsByTopics = testAnalysisResult.TopicsAnalysis.ToDictionary(
            ta => ta.Key,
            ta => (
                TotalPossible: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.TotalPossiblePoints),
                Achieved: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.AchievedPoints)
            ));
        var suggestedChanges = await new RoadmapModificationAdvisor().SuggestRoadmapTopicChnages(
           userModifiedRoadmap.Nodes.Select(n => new Node
           {
               Id = n.Id,
               Title = n.Title,
               Description = n.Description,
           }).ToList(),
           userModifiedRoadmap.Edges,
           pointsByTopics);

        var actualRoadmapNodeStatuses = userModifiedRoadmap.Nodes.ToDictionary(n => n.Id, n => n.Status);
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