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
using SkillMap.Shared.Extensions;
using SkillMap.Shared.Results;

namespace SkillMap.Business.RoadmapTest;

// todo: refcator to more simple cognitive model
public class RoadmapTestService(
    IUserRoadmapsService userRoadmapsService, 
    IRoadmapTestGenerator roadmapTestGenerator, 
    IRoadmapService roadmapService,
    ICustomizedRoadmapsService customizedRoadmapsService,
    IUserRoadmapTestService userRoadmapTestService) : IRoadmapTestService
{
    public async Task<RoadmapTestResultDto> GenerateRoadmapTest(long userId, string roadmapId, RoadmapTestConfigDto config, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        // todo: add config validation
        // todo: refactor on result pattern
        var userRoadmap = await EnsureActiveUserRoadmap(userId, roadmapId, ct);
        var existingTest = await GetOrDefaultInitialUnfinishedTest(userId, userRoadmap.Id, ct);
        if (existingTest != null)
        {
            return existingTest;
        }

        var roadmapResult = await roadmapService.GetRoadmapById(roadmapId, ct);
        if (roadmapResult.IsFailed || !roadmapResult.HasData)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap with id {roadmapId} not found");
        }

        var userSavedRoadmapResult = await customizedRoadmapsService.GetUserModifiedRoadmap(userId, roadmapId, ct);
        if (!userSavedRoadmapResult.IsSuccessful || userSavedRoadmapResult.Data == null)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No customized roadmap found for user {userId} and roadmap {roadmapId}");
        }

        var roadmap = userSavedRoadmapResult.Data;
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
        var topicSettings = targetTopics.Select(t => GetTopicSettings(t, topicsAnalysis[t.Id], config.DifficultyLevel)).ToList();
        var generateTestQuestions = await roadmapTestGenerator.GenerateRoadmapTest(topicSettings, ct);

        var roadmapTest = new RoadmapTestDao
        {
            RoadmapId = roadmapId,
            TopicQuestions = generateTestQuestions,
            TopicSettings = topicSettings.ToDictionary(ts => ts.Topic.Id, ts => ts.Setting),
            TestConfig = config
        };

        var testId = await userRoadmapTestService.SaveUserRoadmapTest(userId, userRoadmap.Id, roadmapId, RoadmapTestType.Initial, roadmapTest, ct);
        return roadmapTest.ToTestResult(testId);
    }

    private async Task<RoadmapTestResultDto?> GetOrDefaultInitialUnfinishedTest(long userId, long userRoadmapId, CancellationToken ct)
    {
        var (hasCompletedTest, roadmapTestId) = await userRoadmapTestService.HasRoadmapCompletedTest(userRoadmapId, RoadmapTestType.Initial, ct);
        if (!hasCompletedTest)
        {
            return null;
        }

        var testIdStr = roadmapTestId.ToString();
        var roadmapTest = await userRoadmapTestService.GetUserRoadmapTest(userId, testIdStr, ct);
        return roadmapTest.ToTestResult(testIdStr);
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

    // todo: what is result already exists?
    // todo: just save selected answers
    public async Task<ComplexTestCheckResult> CheckRoadmapTest(long userId, string testId, RoadmapTestAnswers userAnswers, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // todo: extrcat questions check to exyernal service that create question types and evaluate answers
        var roadmapTest = await userRoadmapTestService.GetUserRoadmapTest(userId, testId, ct);
        var analysisByQuestion = BuildAnalysisByQuestion(roadmapTest, userAnswers);
        var analysisByTopic = GroupAnalysisByTopic(roadmapTest, analysisByQuestion);
        var testAnalysisResult = new RoadmapTestResultsDto(analysisByTopic);
        await userRoadmapTestService.SaveTestAnalysisResult(long.Parse(roadmapTest.UserRoadmapId), testId, testAnalysisResult, ct); // todo: check parse

        return new(); // todo: it is not used anymore
    }

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
        var roadmapTest = await userRoadmapTestService.GetUserRoadmapTest(userId, testId, ct);
        return roadmapTest.ToTestResult(testId);
    }

    public async Task<ComplexTestCheckResult> GetComplexTestCheck(long userId, string testId, CancellationToken ct)
    {
        var roadmapTest = await userRoadmapTestService.GetUserRoadmapTest(userId, testId, ct);
        var roadmapId = roadmapTest.RoadmapId;
        var testAnalysisResult = await userRoadmapTestService.GetTestAnalysisResult(userId, testId, ct);
        var userSavedRoadmapResult = await customizedRoadmapsService.GetUserModifiedRoadmap(userId, roadmapId, ct);
        if (!userSavedRoadmapResult.IsSuccessful || userSavedRoadmapResult.Data == null)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"No customized roadmap found for user {userId} and roadmap {roadmapId}");
        }

        var roadmap = userSavedRoadmapResult.Data;
        var analysisByQuestion = testAnalysisResult.TopicsAnalysis
            .SelectMany(t => t.Value.QuestionsAnalysis)
            .ToDictionary(q => q.Key, q => q.Value);
        var testResults = testAnalysisResult.TopicsAnalysis.ToDictionary(
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

        var actualRoadmapNodeStatuses = roadmap.Nodes.ToDictionary(n => n.Id, n => n.Status);
        var suggestChangesWithDiff = suggestedChanges.Where(sc =>
        {
            var actualStatus = actualRoadmapNodeStatuses.GetOrDefault(sc.Id);
            if (actualStatus == null)
                return false;
            var nodeMarkToLearningStatus = ToLearningStatusString(sc.MarkType);
            return actualStatus != nodeMarkToLearningStatus;
        }).ToList();

        return BuildComplexTestCheckResult(roadmapTest, analysisByQuestion, suggestChangesWithDiff);
    }

    public async Task<SavedUerRoadmap> RebuildRoadmapBasedOnTestResults(long userId, string roadmapId, CancellationToken ct)
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

    private static ComplexTestCheckResult BuildComplexTestCheckResult(RoadmapTestDao roadmapTest, Dictionary<string, QuestionAnalysisResultDto> analysisByQuestion, List<MarkNode> suggestedChanges)
    {
        return new ComplexTestCheckResult
        {
            QuestionResults = roadmapTest.TopicQuestions
                .SelectMany(t => t.Questions)
                .ToDictionary(q => q.Id, q => BuildQuestionResult(q, analysisByQuestion[q.Id])),
            RoadmapId = roadmapTest.RoadmapId,
            ChangesSuggestion = new RoadmapChangesSuggestionsDto
            {
                Suggestions = suggestedChanges.Select(sc => new RoadmapTestSuggestionItemDto
                {
                    LearningItemId = sc.Id,
                    LearningStatus = ToLearningStatusString(sc.MarkType),
                    Title = sc.Title,
                    Description = sc.Description
                }).ToList()
            }
        };
    }

    private static string ToLearningStatusString(NodeMarkType markType)
    {
        return markType switch
        {
            NodeMarkType.Completed => LearningStatus.Completed.ToStatusString(),
            NodeMarkType.NeedsReview => LearningStatus.NotStarted.ToStatusString(),
            NodeMarkType.InProgress => LearningStatus.InProgress.ToStatusString(),
            _ => "Unknown"
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
                    IsSelected = analysis.SelectedAnswerId == answer.Id
                };
            }
            default:
                throw new LearningPlatformException(ErrorCode.INTERNAL_ERROR, $"Unsupported question type {question.Type}");
        }
    }
}
