using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using SkillMap.Business.Roadmaps;
using SkillMap.Business.Roadmaps.Models;
using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Business.UserRoadmaps.Models;
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
    IRoadmapTestGenerator roadmapTestGenerator, 
    IRoadmapService roadmapService,
    ICustomizedRoadmapsService customizedRoadmapsService,
    IUserRoadmapTestService userRoadmapTestService) : IRoadmapTestService
{
    public async Task<RoadmapTestResultDto> CreateInitialRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct)
    {
        var testType = RoadmapTestType.Initial;
        var userRoadmap = await EnsureActiveUserRoadmap(userId, roadmapId, ct);
        // todo: config validation
        // move by simpliest way - always create new test
        // but
        // we have 3 states:
        // 1. no test - create new
        // 2. test in progress - return existing
        // 3. test completed - create new

        var generatedTest = await GenerateRoadmapTest(userId, roadmapId, config, ct);
        var topicsQuestions = generatedTest.Values.SelectMany(v => v.Questions).ToList();
        var topicSettings = generatedTest.ToDictionary(t => t.Key, t => t.Value.CreationSettings);
       
        var roadmapTest = new RoadmapTestDao
        {
            RoadmapId = roadmapId,
            TopicQuestions = topicsQuestions,
            TopicSettings = topicSettings,
            TestConfig = config
        };

        var testId = await userRoadmapTestService.SaveUserRoadmapTest(userId, userRoadmap.Id, roadmapId, testType, roadmapTest, ct);
        return roadmapTest.ToTestResult(testId);
    }

    private async Task<Dictionary<string, (List<TopicQuestionsDto> Questions, TopicQuestionsSettingDto CreationSettings)>> 
        GenerateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var userModifiedRoadmap = await customizedRoadmapsService.GetUserModifiedRoadmap(userId, roadmapId, ct);
        if (!userModifiedRoadmap.IsSuccessful || userModifiedRoadmap.Data == null)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No customized roadmap found for user {userId} and roadmap {roadmapId}");
        }

        var roadmap = userModifiedRoadmap.Data;
        var nodes = roadmap.Nodes.Select(n => new Node
        {
            Id = n.Id,
            Title = n.Title,
            Description = n.Description,
        }).ToList();
        var topics = roadmap.Nodes.Select(n => new Topic(n.Id, n.Title, n.Description)).ToList();
        var coreTopics = new RoadmapAnalyzer().SelectStratifiedCoreTopics(nodes, roadmap.Edges, questionsLimit: config.NumberOfQuestions ?? DefaultNumberOfQuestions);
        var topicsAnalysis = CalculateTopicsAnalysis(topics, config, ct);
        var targetTopics = FilterTopicByTestConfig(topicsAnalysis, topics, config, ct);
        var topicQuestionsCreationSettings = targetTopics.Select(t => GetTopicSettings(t, topicsAnalysis[t.Id], config.DifficultyLevel)).ToList();
        var generateTestQuestions = await roadmapTestGenerator.GenerateRoadmapTest(topicQuestionsCreationSettings, ct);

        return targetTopics.ToDictionary(
            t => t.Id,
            t => (
                Questions: generateTestQuestions.Where(g => g.Id == t.Id).ToList(),
                CreationSettings: topicQuestionsCreationSettings.First(ts => ts.Topic.Id == t.Id).Setting
            ));
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

    private const int MaxQuestionsPerTopic = 3;
    private const int DefaultNumberOfQuestions = 10;
    private const double MinMinutesPerQuestion = 0.5;
    private static HashSet<TestQuestionType> SupportedQuestionTypes = new()
    {
        TestQuestionType.SingleChoice,
    };

    private static Dictionary<string, TopicAnalysis> CalculateTopicsAnalysis(
        List<Topic> topics,
        RoadmapTestConfigDto config,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (topics == null || !topics.Any())
            return new Dictionary<string, TopicAnalysis>();

        int requestedCount = config.NumberOfQuestions ?? DefaultNumberOfQuestions;
        int maxByTime = (int)(config.TimeLimitInMinutes / MinMinutesPerQuestion);
        int maxByTopics = topics.Count * MaxQuestionsPerTopic;
        int finalTargetCount = Math.Min(requestedCount, Math.Min(maxByTime, maxByTopics));

        // 2. CALCULATE DISTRIBUTION (Base + Remainder)
        // Example: 10 questions, 3 topics. 
        // Base = 3. Remainder = 1.
        int baseQuestionsPerTopic = finalTargetCount / topics.Count;
        baseQuestionsPerTopic = Math.Min(baseQuestionsPerTopic, MaxQuestionsPerTopic);
        int extraQuestions = finalTargetCount % topics.Count;
        extraQuestions = Math.Min(extraQuestions, topics.Count);

        var shuffledTopics = topics.OrderBy(x => Guid.NewGuid()).ToList();

        var result = new Dictionary<string, TopicAnalysis>();

        for (int i = 0; i < shuffledTopics.Count; i++)
        {
            var topic = shuffledTopics[i];
            int count = baseQuestionsPerTopic;

            if (i < extraQuestions)
            {
                count++;
            }

            var (priority, coefficient) = GetPriorityAndCoefficient(count);

            result[topic.Id] = new TopicAnalysis
            {
                Id = topic.Id,
                QuestionsCount = count,
                Priority = priority,
                Coefficient = coefficient
            };
        }

        return result;
    }

    private static (string Priority, double Coefficient) GetPriorityAndCoefficient(int count)
    {
        return count switch
        {
            >= 3 => ("High", 1.0),   // Full focus
            2 => ("Medium", 0.7), // Standard focus
            1 => ("Low", 0.4),    // Brief check
            _ => ("None", 0.0)    // Skipped
        };
    }
    private static List<Topic> FilterTopicByTestConfig(
        Dictionary<string, TopicAnalysis> topicsAnalysis,
        List<Topic> topics,
        RoadmapTestConfigDto config,
        CancellationToken ct)
    {
        return topics
            .Where(topic =>
            {
                var analysis = topicsAnalysis.GetOrDefault(topic.Id);
                if (analysis == null || analysis?.QuestionsCount <= 0)
                    return false;

                return true;
            })
            .OrderByDescending(t => topicsAnalysis[t.Id].QuestionsCount)
            .ThenBy(t => t.Name)
            .ToList();
    }
    private static (Topic Topic, TopicQuestionsSettingDto Setting) GetTopicSettings(Topic topic, TopicAnalysis analysis, string difficultyLevel)
    {
       return (topic, new TopicQuestionsSettingDto
       {
           DifficultyLevel = difficultyLevel.FromDifficultyString(),
           QuestionsCount = analysis.QuestionsCount,
           Types = SupportedQuestionTypes.ToList(),
       });
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
    public async Task<SavedUerRoadmap> RebuildRoadmapBasedOnTestResults(long userId, string roadmapId, CancellationToken ct) // bullshit
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

        
        var testResult = await userRoadmapTestService.GetLatestCompletedTestAnalysisResult(userRoadmap.Id, RoadmapTestType.Initial, ct);
        if (testResult == null)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No completed initial test found for user roadmap id {userRoadmap.Id}");
        }

        var userSavedRoadmapResult = await customizedRoadmapsService.GetUserModifiedRoadmap(userId, roadmapId, ct);
        if (!userSavedRoadmapResult.IsSuccessful || userSavedRoadmapResult.Data == null)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No customized roadmap found for user {userId} and roadmap {roadmapId}");
        }

        var roadmap = userSavedRoadmapResult.Data;
        var testResults = testResult.TopicsAnalysis.ToDictionary(
            ta => ta.Key,
            ta => (
                TotalPossible: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.TotalPossiblePoints),
                Achieved: ta.Value.QuestionsAnalysis.Values.Sum(qa => qa.AchievedPoints)
            ));
        var suggestedChanges = await new RoadmapModificationAdvisor().SuggestRoadmapTopicChnages(
           roadmap.Nodes.Select(n => new Node
           {
               Id = n.Id,
               Title = n.Title,
               Description = n.Description,
           }).ToList(),
           roadmap.Edges,
           testResults);

        var nodeStatuses = suggestedChanges.ToDictionary(
            sc => sc.Id,
            sc => sc.MarkType);

        //roadmap.Nodes.ForEach(n =>
        //{
        //    var status = nodeStatuses.GetOrDefault(n.Id);
        //    if (status != null)
        //    {
        //        n.Status = status;

        //    }
        //});
        return roadmap;
    }

    private Dictionary<string, QuestionAnalysisResultDto> EstimateQuestionsAnswers(RoadmapTestDao roadmapTest, RoadmapTestAnswers userAnswers)
    {
        var userAnswersDict = userAnswers.QuestionAnswers.ToDictionary(qa => qa.QuestionId, qa => qa);
        return roadmapTest.TopicQuestions
            .SelectMany(t => t.Questions)
            .ToDictionary(q => q.Id, q => EstimateQuestionAnswer(q, userAnswersDict.GetOrDefault(q.Id)));
    }
}
