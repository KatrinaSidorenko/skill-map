using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Business.UserRoadmaps.Models;
using SkillMap.Business.UserTest;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest;

public class RoadmapTestService(
    IUserRoadmapsService userRoadmapsService, 
    IRoadmapTestGenerator roadmapTestGenerator, 
    IRoadmapService roadmapService,
    IUserTestService userTestsService) : IRoadmapTestService
{
    public async Task<RoadmapTestResultDto> GenerateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        // todo: add config validation
        // todo: refactor on result pattern
        var userRoadmap = await EnsureActiveUserRoadmap(userId, roadmapId, ct);
        var existingTest = await TryGetExistingInitialUnfinishedTest(userRoadmap.Id, ct);
        if (existingTest != null)
        {
            return existingTest;
        }

        var roadmap = await GetRoadmap(roadmapId, ct);
        var topics = roadmap.Nodes.Select(n => new Topic(n.Id, n.Title, n.Description)).ToList();
        var topicsAnalysis = CalculateTopicsAnalysis(topics, config, ct); // todo: not implemented
        var targetTopics = FilterTopicByTestConfig(topicsAnalysis, topics, config, ct); // todo: not implemented
        var topicSettings = targetTopics.Select(t => GetTopicSettings(t, topicsAnalysis[t.Id], config)).ToList();
        var generateTestQuestions = await roadmapTestGenerator.GenerateRoadmapTest(topicSettings, ct);

        var roadmapTest = new RoadmapTestDao
        {
            RoadmapId = roadmapId,
            TopicQuestions = generateTestQuestions,
            TopicSettings = topicSettings.ToDictionary(ts => ts.Topic.Id, ts => ts.Setting),
            TestConfig = config
        };

        var testId = await userTestsService.SaveUserTestWithEmptyResult(userId, userRoadmap.Id, roadmapId, RoadmapTestType.Initial, roadmapTest, ct);
        return roadmapTest.ToTestResult(testId);
    }

    private async Task<RoadmapTestResultDto?> TryGetExistingInitialUnfinishedTest(long userRoadmapId, CancellationToken ct)
    {
        var (exists, savedTest) = await userTestsService.ExistsUnfinishedTest(userRoadmapId, RoadmapTestType.Initial, ct);
        if (!exists || savedTest == null)
        {
            return null;
        }

        return savedTest.ToTestResult(savedTest.Id);
    }

    private async Task<UserRoadmapDto> EnsureActiveUserRoadmap(long userId, string roadmapId, CancellationToken ct)
    {
        var userRoadmapResult = await userRoadmapsService.GetUserRoadmap(userId, roadmapId, ct);
        if (!userRoadmapResult.IsSuccessful)
        {
            throw new LearningPlatformException(userRoadmapResult.ToExceptionResult());
        }

        var userRoadmap = userRoadmapResult.Data;
        if (userRoadmap == null || userRoadmap.IsActive != true)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"User roadmap with id {roadmapId} is null for user {userId}");
        }

        return userRoadmap;
    }

    private async Task<RoadmapDto> GetRoadmap(string roadmapId, CancellationToken ct)
    {
        var roadmapResult = await roadmapService.GetRoadmapById(roadmapId, ct);
        if (!roadmapResult.IsSuccessful || roadmapResult.Data == null)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap with id {roadmapId} not found");
        }

        return roadmapResult.Data;
    }

    private static Dictionary<string, TopicAnalysis> CalculateTopicsAnalysis(List<Topic> topics, RoadmapTestConfigDto config, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        // calculate priorities and coefficients for topics based on config
        return topics.ToDictionary(
            t => t.Id,
            t => new TopicAnalysis { Id = t.Id, Priority = "high", Coefficient = 1.0, QuestionsCount = 1 });
    }
    private static List<Topic> FilterTopicByTestConfig(Dictionary<string, TopicAnalysis> topicsAnalysis,  List<Topic> topics, RoadmapTestConfigDto config, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        // get priorities or coefficeint snad filter
        // filter topics based on config
        return topics.Take(3).ToList();
    }
    private static (Topic Topic, TopicQuestionsSettingDto Setting) GetTopicSettings(Topic topic, TopicAnalysis analysis, RoadmapTestConfigDto config)
    {
        // map config to topic settings
       return (topic, new TopicQuestionsSettingDto
       {
           DifficultyLevel = config.DifficultyLevel.FromDifficultyString(),
           QuestionsCount = analysis.QuestionsCount,
           Types = [TestQuestionType.SingleChoice],
       });
    }

    // todo: what is result already exists?
    public async Task<ComplexTestCheckResult> CheckRoadmapTest(long userId, string testId, RoadmapTestAnswers userAnswers, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // todo: extrcat questions check to exyernal service that create question types and evaluate answers
        var roadmapTest = await userTestsService.GetUserTest(userId, testId, ct);
        var analysisByQuestion = BuildAnalysisByQuestion(roadmapTest, userAnswers);
        var analysisByTopic = GroupAnalysisByTopic(roadmapTest, analysisByQuestion);
        var testAnalysisResult = new RoadmapTestResultsDto(analysisByTopic);
        await userTestsService.SaveTestAnalysisResult(long.Parse(roadmapTest.UserRoadmapId), testId, testAnalysisResult, ct); // todo: check parse

        return BuildComplexTestCheckResult(roadmapTest, analysisByQuestion);
    }

   // public async Task

    // analysis for each question
    private QuestionAnalysisResultDto AnalyzeQuestionAnswer(QuestionDto questionDto, QuestionAnswer? userAnswer)
    {
        switch(questionDto.Type)
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
                throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, $"Unsupported question type {questionDto.Type}");
        }
    }
    public async Task<RoadmapTestResultDto> GetUserTest(long userId, string testId, CancellationToken ct)
    {
        var roadmapTest = await userTestsService.GetUserTest(userId, testId, ct);
        return roadmapTest.ToTestResult(testId);
    }

    // todo: remove duplication with CheckRoadmapTest
    public async Task<ComplexTestCheckResult> GetComplexTestCheck(long userId, string testId, CancellationToken ct)
    {
        var roadmapTest = await userTestsService.GetUserTest(userId, testId, ct);
        var testAnalysisResult = await userTestsService.GetTestAnalysisResult(userId, testId, ct);
        var analysisByQuestion = testAnalysisResult.TopicsAnalysis
            .SelectMany(t => t.Value.QuestionsAnalysis)
            .ToDictionary(q => q.Key, q => q.Value);

        return BuildComplexTestCheckResult(roadmapTest, analysisByQuestion);
    }

    private Dictionary<string, QuestionAnalysisResultDto> BuildAnalysisByQuestion(RoadmapTestDao roadmapTest, RoadmapTestAnswers userAnswers)
    {
        var userAnswersDict = userAnswers.QuestionAnswers.ToDictionary(qa => qa.QuestionId, qa => qa);
        return roadmapTest.TopicQuestions
            .SelectMany(t => t.Questions)
            .ToDictionary(q => q.Id, q => AnalyzeQuestionAnswer(q, userAnswersDict.GetOrDefault(q.Id)));
    }

    private static Dictionary<string, TopicAnswersAnalysisDto> GroupAnalysisByTopic(RoadmapTestDao roadmapTest, Dictionary<string, QuestionAnalysisResultDto> analysisByQuestion)
    {
        return roadmapTest.TopicQuestions.ToDictionary(
            t => t.Id,
            t => new TopicAnswersAnalysisDto
            {
                QuestionsAnalysis = t.Questions.ToDictionary(q => q.Id, q => analysisByQuestion[q.Id])
            });
    }

    private static ComplexTestCheckResult BuildComplexTestCheckResult(RoadmapTestDao roadmapTest, Dictionary<string, QuestionAnalysisResultDto> analysisByQuestion)
    {
        return new ComplexTestCheckResult
        {
            QuestionResults = roadmapTest.TopicQuestions
                .SelectMany(t => t.Questions)
                .ToDictionary(q => q.Id, q => BuildQuestionResult(q, analysisByQuestion[q.Id]))
        };
    }

    private static TestQuestionResult BuildQuestionResult(QuestionDto question, QuestionAnalysisResultDto analysis)
    {
        return new TestQuestionResult
        {
            Type = analysis.QuestionType.ToQuestionTypeString(),
            TotalPossiblePoints = analysis.TotalPossiblePoints,
            AchievedPoints = analysis.AchievedPoints,
            IsCorrect = analysis.AchievedPoints >= analysis.TotalPossiblePoints,
            QuestionId = question.Id,
            Text = question.Text,
            AnswerDetails = question.Answers
                .Select(answer => BuildAnswerDetail(question, answer, analysis))
                .ToDictionary(ad => ad.AnswerId, ad => ad)
        };
    }

    private static AnswerDetail BuildAnswerDetail(QuestionDto question, AnswerDto answer, QuestionAnalysisResultDto analysis)
    {
        switch (question.Type)
        {
            case TestQuestionType.SingleChoice:
            {
                var singleChoiceAnalysis = analysis as SingleAnswerQuestionAnalysisResultDto;
                return new SingleChoiceAnswerDetail
                {
                    Text = answer.Text,
                    AnswerId = answer.Id,
                    IsCorrect = answer.IsCorrect,
                    IsSelected = singleChoiceAnalysis != null && singleChoiceAnalysis.SelectedAnswerId == answer.Id
                };
            }
            default:
                throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, $"Unsupported question type {question.Type}");
        }
    }
}
