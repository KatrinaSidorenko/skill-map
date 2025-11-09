using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using Microsoft.Extensions.Caching.Memory;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;
using System.Security.Cryptography.X509Certificates;

namespace SkillMap.Business.RoadmapTest;

public class TopicAnalysis
{
    public string Priority { get; set; }
    public double Coefficient { get; set; }
    public string Id { get; set; }
    public int QuestionsCount { get; set; }
}
public class RoadmapTestService(
    IUserRoadmapsService userRoadmapsService, 
    IRoadmapTestGenerator roadmapTestGenerator, 
    IRoadmapService roadmapService, 
    IMemoryCache memoryCache) : IRoadmapTestService
{
    private const string RoadmapTestCacheKeyPrefix = "RoadmapTest_";

    public async Task<RoadmapTestResult> GenerateRoadmapTest(long userId, string roadmapId, RoadmapTestConfig config, CancellationToken ct)
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
            Id = $"{userId}_{roadmapId}",
            RoadmapId = roadmapId,
            TopicQuestions = generateTestQuestions,
            TopicSettings = topicSettings.ToDictionary(ts => ts.Topic.Id, ts => ts.Setting)
        }; // todo: save to db 

        // for test purposes save to memory cache
        memoryCache.Set(RoadmapTestCacheKeyPrefix + roadmapTest.Id, roadmapTest, TimeSpan.FromHours(1));

        return new RoadmapTestResult
        {
            TestId = roadmapTest.Id,
            Questions = roadmapTest.TopicQuestions.SelectMany(t => t.Questions.Select(q => new QuestionResult
            {
                Id = q.Id,
                TopicId = t.Id,
                Text = q.Text,
                Type = q.Type.ToQuestionTypeString(),
                Answers = q.Answers.Select(a => new AnswerResult
                {
                    Id = a.Id,
                    Text = a.Text
                }).ToList()
            })).ToList()
        };
    }

    private async Task<Dictionary<string, TopicAnalysis>> CalculateTopicsAnalysis(List<Topic> topics, RoadmapTestConfig config, CancellationToken ct)
    {
        // calculate priorities and coefficients for topics based on config
        return await Task.FromResult(topics.ToDictionary(t => t.Id, t => new TopicAnalysis { Id = t.Id, Priority = "high", Coefficient = 1.0, QuestionsCount = 1 }));
    }
    private async Task<List<Topic>> FilterTopicByTestConfig(Dictionary<string, TopicAnalysis> topicsAnalysis,  List<Topic> topics, RoadmapTestConfig config, CancellationToken ct)
    {
        // get priorities or coefficeint snad filter
        // filter topics based on config
        return await Task.FromResult(topics.Take(3).ToList());
    }
    private (Topic Topic, TopicQuestionSetting Setting) GetTopicSettings(Topic topic, TopicAnalysis analysis, RoadmapTestConfig config, CancellationToken ct)
    {
        // map config to topic settings
       return (topic, new TopicQuestionSetting
       {
           DifficultyLevel = config.DifficultyLevel.FromDifficultyString(),
           QuestionsCount = analysis.QuestionsCount,
           Types = [TestQuestionType.SingleChoice],
       });
    }

    public async Task<AnswersCheckResult> CheckRoadmapTest(long userId, string testId, RoadmapTestAnswers userAnswers, CancellationToken ct)
    {
        //todo: get test from db or cache
        if (!memoryCache.TryGetValue<RoadmapTestDao>(RoadmapTestCacheKeyPrefix + testId, out var roadmapTest))
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap test with id {testId} not found");
        }

        var targetTopicQuestions = roadmapTest.TopicQuestions.ToDictionary(t => t.Id, 
            t => t.Questions.ToDictionary(q => q.Id, q => q.Answers.ToDictionary(a => a.Id, a => a.IsCorrect)));
        var userAnswersDict = userAnswers.QuestionAnswers.ToDictionary(qa => qa.QuestionId, qa => qa);
        var analysisByTopic = roadmapTest.TopicQuestions.ToDictionary(t => t.Id, t =>
        {
            var questionsAnalysis = t.Questions.ToDictionary(q => q.Id, q => AnalyzeQuestionAnswer(q, userAnswersDict.GetOrDefault(q.Id)));
            return new TopicAnswersAnalysis
            {
                QuestionsAnalysis = questionsAnalysis
            };
        });
        var questionsAnalysis = analysisByTopic.SelectMany(t => t.Value.QuestionsAnalysis).ToDictionary(qa => qa.Key, qa => qa.Value);

        //todo: save analysis result to db

        return new AnswersCheckResult
        {
            QuestionResults = questionsAnalysis.ToDictionary(qa => qa.Key, qa =>
            {
                return qa.Value switch
                {
                    SingleAnswerQuestionAnalysisResult single => new CheckedSingleAnswerQuestion
                    {
                        QuestionId = qa.Key,
                        AchievedPoints = single.AchievedPoints,
                        TotalPossiblePoints = single.TotalPossiblePoints,
                        IsCorrect = single.IsCorrect,
                        CorrectAnswerId = single.CorrectAnswerId,
                    } as CheckedQuestion,
                    _ => throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, $"Unsupported question type {qa.Value.QuestionType}"),
                };
            })
        };
    }

    // analysis for each question
    private QuestionAnalysisResult AnalyzeQuestionAnswer(QuestionDto questionDto, QuestionAnswer? userAnswer)
    {
        switch(questionDto.Type)
        {
            case TestQuestionType.SingleChoice:
                {
                    var singleChoiceAnswer = userAnswer as SingleChoiceAnswer;
                    var correctAnswer = questionDto.Answers.FirstOrDefault(a => a.IsCorrect);
                    var isCorrect = correctAnswer != null && singleChoiceAnswer != null && singleChoiceAnswer.SelectedAnswerId == correctAnswer.Id;
                    return new SingleAnswerQuestionAnalysisResult
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
