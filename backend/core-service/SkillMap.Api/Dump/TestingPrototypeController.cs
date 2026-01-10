//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;

//namespace SkillMap.Api.Dump;


//public record Topic(string Id, string Name, string Description);
//public record QuestionsSettings(string Complexity, int QuestionsCount, string QuestionType);

//public class Question
//{
//    public string Id { get; set; }
//    public string Text { get; set; }
//    public List<Answer> Answers { get; set; }
//}

//public record Answer(string Id, string Text, bool IsCorrect);


//public class TopicQuestions
//{
//    public List<Question> Questions { get; set; }
//}

//public class RoadmapTest
//{
//    public List<RoadmapTopic> Topics { get; set; }
//}

//public class RoadmapTopic
//{
//    public Topic Topic { get; set; }
//    public TopicQuestions TopicQuestions { get; set; }
//    public QuestionsSettings QuestionsSettings { get; set; }
//}


//public interface IQuestionsGenerator
//{
//    Task<TopicQuestions> GenerateQuestions(Topic topic, QuestionsSettings settings, CancellationToken ct);
//}

//public class QuestionsGenerator : IQuestionsGenerator
//{
//    public async Task<TopicQuestions> GenerateQuestions(Topic topic, QuestionsSettings settings, CancellationToken ct)
//    {
//        // here should be logic to generate questions based on topic and settings
//        // for demo purposes, returning a sample question
//        var sampleQuestion = new Question
//        {
//            Id = "q1",
//            Text = "What does HTTP stand for?",
//            Answers = new List<Answer>
//            {
//                new Answer("a1", "HyperText Transfer Protocol", true),
//                new Answer("a2", "HighText Transfer Protocol", false),
//                new Answer("a3", "HyperText Transmission Protocol", false)
//            }
//        };
//        var topicQuestions = new TopicQuestions
//        {
//            Questions = new List<Question> { sampleQuestion }
//        };
//        return await Task.FromResult(topicQuestions);
//    }
//}

//public class QuestionsComplexity
//{
//    public const string Easy = "easy";
//    public const string Medium = "medium";
//    public const string Hard = "hard";
//}

//public class QuestionType
//{
//    public const string SingleChoice = "single-choice";
//    public const string MultipleChoice = "multiple-choice";
//    public const string Text = "text";
//}

using LearningPlatform.Roadmap.Business.Contracts;
using LearningPlatform.RoadmapTests.Contracts;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Business.UserTest;
using SkillMap.Shared.Results;

[Route("api/[controller]")]
[ApiController]
public class TestingPrototypeController(
    IUserRoadmapsService userRoadmapsService,
    IRoadmapTestGenerator roadmapTestGenerator,
    IRoadmapService roadmapService,
    IUserRoadmapTestService userTestsService) : ControllerBase
{
    // 1. task 1: create GET question and answers for 1 topic
    // desired complexity
    // questions count
    // type of questions (multiple choice, single choice, text)

    // what info we need for getting questions?
    // topic description + name
    // desired complexity
    // questions count
    //private readonly IQuestionsGenerator _questionsGenerator;
    //private readonly IMemoryCache _memoryCache;
    //public TestingPrototypeController(IQuestionsGenerator questionsGenerator, IMemoryCache memoryCache)
    //{
    //    _questionsGenerator = questionsGenerator;
    //    _memoryCache = memoryCache;
    //}

    //private static Topic SampleTopic = new(
    //    Id: "internet-basics-test",
    //    Name: "Internet",
    //    Description: "Fundamental concepts like HTTP, HTTPS, protocols and so on"
    //);

    //// todo: extract to enums
    //private static int SampleQuestionsCount = 1;
    //private static string CacheKey_LatestTopicQuestions = "latest-topic-questions";

    //[HttpGet("generate-questions")]
    //public async Task<IActionResult> GenerateQuestions(CancellationToken ct)
    //{
    //    // here is something that can return topicquestions based on provided topic and settings
    //    var settings = new QuestionsSettings(
    //        Complexity: QuestionsComplexity.Easy,
    //        QuestionsCount: SampleQuestionsCount,
    //        QuestionType: QuestionType.SingleChoice
    //    );

    //    // todo: estimated each topic in roadmap in order to fit into time bounds

    //    var topicQuestions = await _questionsGenerator.GenerateQuestions(SampleTopic, settings, ct);
    //    // save in prod to db
    //    var roadmapTest = new RoadmapTest
    //    {
    //        Topics = new List<RoadmapTopic>
    //        {
    //            new RoadmapTopic
    //            {
    //                Topic = SampleTopic,
    //                TopicQuestions = topicQuestions,
    //                QuestionsSettings = settings
    //            }
    //        }
    //    };
    //    _memoryCache.Set(CacheKey_LatestTopicQuestions, roadmapTest);

    //    return Ok(topicQuestions);
    //}

    //// 2. task 2: create check answer for 1 topic
    //// correct or not
    //[HttpGet("check-question-answer")]
    //public async Task<IActionResult> CheckQuestionAnswer(string questionId, string answerId, CancellationToken ct)
    //{
    //    //todo: all this logic should be extracted to service

    //    // get latest generated questions from memory cache
    //    if (!_memoryCache.TryGetValue<RoadmapTest>(CacheKey_LatestTopicQuestions, out var roadmapTest))
    //    {
    //        return BadRequest("No generated questions found. Please generate questions first.");
    //    }
    //    // find question by id
    //    var question = roadmapTest.Topics
    //        .SelectMany(t => t.TopicQuestions.Questions)
    //        .FirstOrDefault(q => q.Id == questionId);
    //    if (question == null)
    //    {
    //        return NotFound("Question not found.");
    //    }
    //    // find answer by id
    //    var answer = question.Answers.FirstOrDefault(a => a.Id == answerId);
    //    if (answer == null)
    //    {
    //        return NotFound("Answer not found.");
    //    }
    //    // check if the answer is correct
    //    var isCorrect = answer.IsCorrect;
    //    return Ok(new { QuestionId = questionId, AnswerId = answerId, IsCorrect = isCorrect });
    //}

    // task 3: rebuild roadmap based on answers


    [HttpGet("analyze-roadmap")]
    public async Task<IActionResult> AnalyzeRoadmap(CancellationToken ct)
    {
        var userId = 7;
        var roadmapId = "ff8f4fbcaa164032afcf7eeea6c2d888";

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

        var roadmapResult = await roadmapService.GetRoadmapById(roadmapId, ct);
        if (roadmapResult.IsFailed || !roadmapResult.HasData)
        {
            throw new LearningPlatformException(ErrorCode.NOT_FOUND, $"Roadmap with id {roadmapId} not found");
        }

        var roadmap = roadmapResult.Data;
        //var coreTopics = new RoadmapAnalyzer().FindCoreTopics(roadmap.Nodes, roadmap.Edges);

        var coreTopics2 = new RoadmapAnalyzer().SelectStratifiedCoreTopics(roadmap.Nodes, roadmap.Edges, questionsLimit: 10);
        var random = new Random();
        var testResults = coreTopics2.Select(t => new KeyValuePair<string, (int, int)>(t.Id, (2, random.Next(2, 2)))).ToDictionary(kv => kv.Key, kv => kv.Value);
        var suggestedChanges = await new RoadmapModificationAdvisor().SuggestRoadmapTopicChnages(
            roadmap.Nodes,
            roadmap.Edges,
            testResults);
        var completed = suggestedChanges.Where(n => n.MarkType == NodeMarkType.Completed).ToList();
        var rebuildedRoadmap = new RoadmapRebuilder().RebuildRemainingRoadmap(
            suggestedChanges,
            roadmap.Edges);
        return Ok(rebuildedRoadmap);
    }
}
