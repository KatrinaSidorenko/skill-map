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
using LearningPlatform.Roadmap.Business.Contracts.Models;
using LearningPlatform.RoadmapTests.Contracts;

using Microsoft.AspNetCore.Mvc;

using SkillMap.Business.RoadmapTest.Helpers;
using SkillMap.Business.RoadmapTest.Models;
using SkillMap.Business.RoadmapTest.TopicQuestionComposers;
using SkillMap.Business.UserRoadmaps;
using SkillMap.Business.UserTest;
using SkillMap.Shared.Results;

[Route("api/[controller]")]
[ApiController]
public class TestingPrototypeController(
    IUserRoadmapsService userRoadmapsService,
    ITopicQuestionsGenerator roadmapTestGenerator,
    IRoadmapService roadmapService) : ControllerBase
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
            throw new LearningPlatformException(ErrorCode.NOTFOUND, $"User roadmap with id {roadmapId} is null for user {userId}");
        }

        var roadmapResult = await roadmapService.GetRoadmapById(roadmapId, ct);
        if (roadmapResult.IsFailed || !roadmapResult.HasData)
        {
            throw new LearningPlatformException(ErrorCode.NOTFOUND, $"Roadmap with id {roadmapId} not found");
        }

        var roadmap = roadmapResult.Data;
        //var coreTopics = new RoadmapAnalyzer().FindCoreTopics(roadmap.Nodes, roadmap.Edges);

        //var coreTopics2 = new StratifiedRoadmapTopicsSelector().SelectStratifiedCoreTopics(roadmap.Nodes, roadmap.Edges, questionsLimit: 10);
        //var random = new Random();
        //var testResults = coreTopics2.Select(t => new KeyValuePair<string, (int, int)>(t.Id, (2, random.Next(2, 2)))).ToDictionary(kv => kv.Key, kv => kv.Value);
        //var suggestedChanges = await new RoadmapModificationAdvisor().SuggestRoadmapTopicChnages(
        //    roadmap.Nodes,
        //    roadmap.Edges,
        //    testResults);
        //var completed = suggestedChanges.Where(n => n.MarkType == NodeMarkType.Finished).ToList();
        //var rebuildedRoadmap = new RoadmapRebuilder().RebuildRemainingRoadmap(
        //    suggestedChanges,
        //    roadmap.Edges);
        return Ok();
    }

    [HttpGet("select-test-topics")]
    public async Task<IActionResult> SelectTestTopics(CancellationToken ct)
    {
        // ─────────────────────────────────────────────────────────────────────────────
        // Internet Learning Roadmap — Test Data
        // Matches the DAG analysed in the assessment selection walkthrough.
        // Vertex identifiers are kept as "v1"–"v18" so they align directly with
        // the formal notation used throughout the paper.
        // ─────────────────────────────────────────────────────────────────────────────

                var nodes = new List<Node>
        {
            // Level 0 — no prerequisites
            new Node { Id = "v1",  Title = "Binary and Number Systems",
                        Description = "Representation of data in binary, decimal and hexadecimal; "  +
                                      "bitwise operations and their role in low-level data processing." },
            new Node { Id = "v2",  Title = "Computer Hardware Basics",
                        Description = "Physical components of a computer system: CPU, memory, storage " +
                                      "devices, buses and their interactions." },

            // Level 1
            new Node { Id = "v3",  Title = "Operating System Concepts",
                        Description = "Role of the operating system; process and memory management; "  +
                                      "system calls and the user–kernel boundary." },
            new Node { Id = "v4",  Title = "Data Representation",
                        Description = "Encoding of text, images and files in binary form; "            +
                                      "character sets, compression basics and file formats." },

            // Level 2
            new Node { Id = "v5",  Title = "Network Fundamentals",
                        Description = "Core concepts of computer networks: nodes, links, network "     +
                                      "topologies, bandwidth, latency and the OSI reference model." },
            new Node { Id = "v6",  Title = "File Systems and I/O",
                        Description = "Organisation of data on storage devices; file system hierarchies; " +
                                      "I/O abstractions and buffering strategies." },

            // Level 3
            new Node { Id = "v7",  Title = "IP Addressing and Subnetting",
                        Description = "IPv4 and IPv6 address structure; CIDR notation; subnet masks "  +
                                      "and the division of address space into subnetworks." },
            new Node { Id = "v8",  Title = "Packet Switching and Routing",
                        Description = "Principles of packet-switched networks; routing algorithms; "    +
                                      "forwarding tables and path selection across internetworks." },

            // Level 4
            new Node { Id = "v9",  Title = "TCP/IP Protocol Stack",
                        Description = "Layered architecture of TCP/IP; responsibilities of each layer; " +
                                      "encapsulation, segmentation and reliable delivery via TCP." },
            new Node { Id = "v10", Title = "DNS — Domain Name System",
                        Description = "Hierarchical namespace of the Internet; resolution process from " +
                                      "stub resolver to authoritative server; record types and caching." },

            // Level 5
            new Node { Id = "v11", Title = "HTTP and HTTPS",
                        Description = "Hypertext Transfer Protocol: request–response model, methods, "  +
                                      "status codes, headers and the transition to encrypted HTTPS." },
            new Node { Id = "v12", Title = "Sockets and Ports",
                        Description = "Socket abstraction for network communication; port numbers; "    +
                                      "TCP and UDP socket lifecycles and the Berkeley socket API." },

            // Level 6
            new Node { Id = "v13", Title = "Web Browsers and the Request–Response Cycle",
                        Description = "Browser architecture; URL parsing; DNS lookup; TCP handshake; "  +
                                      "HTTP request dispatch; rendering pipeline overview." },
            new Node { Id = "v14", Title = "TLS and Certificates",
                        Description = "Transport Layer Security handshake; symmetric and asymmetric "   +
                                      "cryptography; X.509 certificates and certificate authorities." },
            new Node { Id = "v15", Title = "REST APIs",
                        Description = "Representational State Transfer constraints; resource modelling; " +
                                      "HTTP verbs as CRUD operations; JSON payloads and status codes." },

            // Level 7
            new Node { Id = "v16", Title = "Authentication and Session Management",
                        Description = "Cookie-based and token-based authentication; session lifecycle; " +
                                      "OAuth 2.0 flows and JWT structure." },
            new Node { Id = "v17", Title = "Web Application Architecture",
                        Description = "Client–server separation; MVC and layered patterns; stateless "  +
                                      "backends; CDN usage and horizontal scalability basics." },

            // Level 8 — target / sink
            new Node { Id = "v18", Title = "Web Security",
                        Description = "Common vulnerability classes: XSS, CSRF, SQL injection; "        +
                                      "HTTPS enforcement; Content Security Policy and secure headers." }
        };

        // ─────────────────────────────────────────────────────────────────────────────
        // Edges — directed prerequisite relations (source must be mastered before target)
        // ─────────────────────────────────────────────────────────────────────────────

            var edges = new List<Edge>
    {
        // V1 → dependents
        new Edge { Id = "e01", Source = "v1",  Target = "v4"  },
        new Edge { Id = "e02", Source = "v1",  Target = "v3"  },   // V3 also needs V2

        // V2 → dependents
        new Edge { Id = "e03", Source = "v2",  Target = "v3"  },
        new Edge { Id = "e04", Source = "v2",  Target = "v5"  },

        // V3 → dependents
        new Edge { Id = "e05", Source = "v3",  Target = "v5"  },
        new Edge { Id = "e06", Source = "v3",  Target = "v6"  },

        // V4 → dependents
        new Edge { Id = "e07", Source = "v4",  Target = "v5"  },

        // V5 → dependents
        new Edge { Id = "e08", Source = "v5",  Target = "v7"  },
        new Edge { Id = "e09", Source = "v5",  Target = "v8"  },

        // V6 → dependents
        new Edge { Id = "e10", Source = "v6",  Target = "v8"  },

        // V7 → dependents
        new Edge { Id = "e11", Source = "v7",  Target = "v9"  },
        new Edge { Id = "e12", Source = "v7",  Target = "v10" },

        // V8 → dependents
        new Edge { Id = "e13", Source = "v8",  Target = "v9"  },

        // V9 → dependents
        new Edge { Id = "e14", Source = "v9",  Target = "v11" },
        new Edge { Id = "e15", Source = "v9",  Target = "v12" },

        // V10 → dependents
        new Edge { Id = "e16", Source = "v10", Target = "v11" },

        // V11 → dependents
        new Edge { Id = "e17", Source = "v11", Target = "v13" },
        new Edge { Id = "e18", Source = "v11", Target = "v14" },
        new Edge { Id = "e19", Source = "v11", Target = "v15" },

        // V12 → dependents
        new Edge { Id = "e20", Source = "v12", Target = "v15" },

        // V13 → dependents
        new Edge { Id = "e21", Source = "v13", Target = "v16" },
        new Edge { Id = "e22", Source = "v13", Target = "v17" },

        // V14 → dependents
        new Edge { Id = "e23", Source = "v14", Target = "v16" },
        new Edge { Id = "e24", Source = "v14", Target = "v18" },

        // V15 → dependents
        new Edge { Id = "e25", Source = "v15", Target = "v17" },

        // V16 → dependents
        new Edge { Id = "e26", Source = "v16", Target = "v18" },

        // V17 → dependents
        new Edge { Id = "e27", Source = "v17", Target = "v18" }
    };
        var topicsSelector = new BaseTopicQuestionComposer(null, null, null);
        var config = new RoadmapTestConfigDto
        {
            NumberOfQuestions = 10,
            DifficultyLevel = "medium"
        };
        return Ok(await topicsSelector.GenerateRoadmapTestQuestions(nodes, edges, config, ct));
        //var roadmapId = "ff8f4fbcaa164032afcf7eeea6c2d888";
        //var topics = await roadmapTestGenerator.SelectTestTopics(roadmapId, questionsLimit: 10, ct);
        //return Ok(topics);
    }
}