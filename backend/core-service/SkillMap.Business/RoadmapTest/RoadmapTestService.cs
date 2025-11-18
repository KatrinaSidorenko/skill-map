using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmaps;
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
        var userRoadmapResult = await userRoadmapsService.GetUserRoadmap(userId, roadmapId, ct);
        if (!userRoadmapResult.IsSuccessful)
        {
            throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR);
        }
        if (userRoadmapResult.Data == null || userRoadmapResult.Data.IsActive != true)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"User roadmap with id {roadmapId} is null for user {userId}");
        }

        var (exists, savedTest) = await userTestsService.ExistsUnfinishedTest(userRoadmapResult.Data.Id, RoadmapTestType.Initial, ct);
        if (exists && savedTest != null)
        {
            return savedTest.ToTestResult(savedTest.Id);
        }

        var roadmapResult  = await roadmapService.GetRoadmapById(roadmapId, ct);
        if (!roadmapResult.IsSuccessful || roadmapResult.Data == null)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap with id {roadmapId} not found");
        }

        var topics = roadmapResult.Data.Nodes.Select(n => new Topic(n.Id, n.Title, n.Description)).ToList();
        var topicsAnalysis = await CalculateTopicsAnalysis(topics, config, ct);
        var targetTopics = await FilterTopicByTestConfig(topicsAnalysis, topics, config, ct);
        var topicSettings = targetTopics.Select(t => GetTopicSettings(t, topicsAnalysis[t.Id], config, ct)).ToList();
        var generateTestQuestions = await roadmapTestGenerator.GenerateRoadmapTest(topicSettings, ct);

        var roadmapTest = new RoadmapTestDao
        {
            RoadmapId = roadmapId,
            TopicQuestions = generateTestQuestions,
            TopicSettings = topicSettings.ToDictionary(ts => ts.Topic.Id, ts => ts.Setting),
            TestConfig = config
        }; 

        var testId = await userTestsService.SaveUserTestWithResult(userId, userRoadmapResult.Data.Id, roadmapId, RoadmapTestType.Initial, roadmapTest, ct);
        return roadmapTest.ToTestResult(testId);
    }

    private async Task<Dictionary<string, TopicAnalysis>> CalculateTopicsAnalysis(List<Topic> topics, RoadmapTestConfigDto config, CancellationToken ct)
    {
        // calculate priorities and coefficients for topics based on config
        return await Task.FromResult(topics.ToDictionary(t => t.Id, t => new TopicAnalysis { Id = t.Id, Priority = "high", Coefficient = 1.0, QuestionsCount = 1 }));
    }
    private async Task<List<Topic>> FilterTopicByTestConfig(Dictionary<string, TopicAnalysis> topicsAnalysis,  List<Topic> topics, RoadmapTestConfigDto config, CancellationToken ct)
    {
        // get priorities or coefficeint snad filter
        // filter topics based on config
        return await Task.FromResult(topics.Take(3).ToList());
    }
    private (Topic Topic, TopicQuestionsSettingDto Setting) GetTopicSettings(Topic topic, TopicAnalysis analysis, RoadmapTestConfigDto config, CancellationToken ct)
    {
        // map config to topic settings
       return (topic, new TopicQuestionsSettingDto
       {
           DifficultyLevel = config.DifficultyLevel.FromDifficultyString(),
           QuestionsCount = analysis.QuestionsCount,
           Types = [TestQuestionType.SingleChoice],
       });
    }

    public async Task<AnswersCheckResult> CheckRoadmapTest(long userId, string testId, RoadmapTestAnswers userAnswers, CancellationToken ct)
    {
        var roadmapTest = await userTestsService.GetUserTest(userId, testId, ct);
        var userAnswersDict = userAnswers.QuestionAnswers.ToDictionary(qa => qa.QuestionId, qa => qa);
        var analysisByTopic = roadmapTest.TopicQuestions.ToDictionary(t => t.Id, t =>
        {
            var questionsAnalysis = t.Questions.ToDictionary(q => q.Id, q => AnalyzeQuestionAnswer(q, userAnswersDict.GetOrDefault(q.Id)));
            return new TopicAnswersAnalysisDto
            {
                QuestionsAnalysis = questionsAnalysis
            };
        });

        var testAnalysisResult = new RoadmapTestResultsDto(analysisByTopic);
        await userTestsService.SaveTestAnalysisResult(userId, testId, testAnalysisResult, ct);
        return testAnalysisResult.ToCheckedResults();
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
}
