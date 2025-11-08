using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using Microsoft.Extensions.Caching.Memory;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Shared.Results;

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
            RoadmapId = roadmapId,
            Questions = generateTestQuestions,
            TopicSettings = topicSettings.ToDictionary(ts => ts.Topic.Id, ts => ts.Setting)
        }; // todo: save to db 

        // for test purposes save to memory cache
        memoryCache.Set(RoadmapTestCacheKeyPrefix + $"{userId}_{roadmapId}", roadmapTest, TimeSpan.FromHours(1));

        return new RoadmapTestResult
        {
            Questions = roadmapTest.Questions.SelectMany(t => t.Questions.Select(q => new QuestionResult
            {
                Id = q.Id,
                TopicId = t.Id,
                Text = q.Text,
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
           Type = TestQuestionType.SingleChoice,
       });
    }

}
