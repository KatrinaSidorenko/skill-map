using LearningPlatform.RoadmapTests.Contracts;
using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application;
using LearningPlatform.RoadmapTests.Service.Application.Mappers;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Core;
using LearningPlatform.RoadmapTests.Service.Persistence.Abstractions;
using LearningPlatform.RoadmapTests.Service.Persistence.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillMap.Shared.Extensions;
using AnswerDto = LearningPlatform.RoadmapTests.Service.Application.Models.AnswerDto;
using QuestionDto = LearningPlatform.RoadmapTests.Service.Application.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.Api;

[Route("api/[controller]")]
[ApiController]
public class SimpleTestsController : ControllerBase
{
    private readonly ITopicQuestionsRepository _topicQuestionsRepository;
    public SimpleTestsController(ITopicQuestionsRepository topicQuestionsRepository)
    {
        _topicQuestionsRepository = topicQuestionsRepository;
    }

    //var topicName = "Dependency Injection";
    //var topicDescription = "A design pattern used to achieve Inversion of Control by injecting dependencies rather than creating them directly.";
    //var difficultyLevel = Difficulty.Easy.ToString();
    //var topicEntity = topicQuestionsDto.ToTopicEntity(topicName, topicDescription);
    //var topicQuestionsEntity = topicQuestionsDto.ToQuestionEntities(difficultyLevel);

//    {
//  "id": "backend-di",
//  "questions": [
//    {
//      "id": "backend-di-q1",
//      "text": "What is the primary goal of dependency injection?",
//      "type": 0,
//      "answers": [
//        {
//          "id": "backend-di-q1-a1",
//          "text": "To invert control and supply dependencies from the outside",
//          "isCorrect": true
//        },
//        {
//          "id": "backend-di-q1-a2",
//          "text": "To create dependencies inside the class using new",
//          "isCorrect": false
//        },
//        {
//    "id": "backend-di-q1-a3",
//          "text": "To optimize for speed by avoiding interfaces",
//          "isCorrect": false
//        }
//      ]
//    },
//    {
//    "id": "backend-di-q2",
//      "text": "Which technique is commonly used in DI to provide dependencies through a constructor?",
//      "type": 0,
//      "answers": [
//        {
//        "id": "backend-di-q2-a1",
//          "text": "Constructor injection",
//          "isCorrect": true
//        },
//        {
//        "id": "backend-di-q2-a2",
//          "text": "Global singleton access",
//          "isCorrect": false
//        },
//        {
//        "id": "backend-di-q2-a3",
//          "text": "Direct instantiation with new inside the consumer",
//          "isCorrect": false
//        }
//      ]
//    }
//  ]
//}
    [HttpPost]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var topicName = "Dependency Injection";
        var topicDescription = "A design pattern used to achieve Inversion of Control by injecting dependencies rather than creating them directly.";
        var difficultyLevel = Difficulty.Easy.ToString();

        var topicEntity = new TopicEntity
        {
            ExternalId = "backend-di",
            Name = topicName,
            Description = topicDescription
        };

        var topicId = await _topicQuestionsRepository.InsertTopicAsync(topicEntity, ct);

        return Ok(topicId);
    }

    [HttpPost("questions")]
    public async Task<IActionResult> CreateQuestions([FromQuery]long topicId, CancellationToken ct)
    {
        var difficultyLevel = Difficulty.Easy.ToDifficultyString();
        var questions = new List<QuestionEntity>
        {
            new QuestionEntity
            {
                ExternalId = "backend-di-q1",
                Text = "What is the primary goal of dependency injection?",
                Difficulty = difficultyLevel,
                Type = TestQuestionType.SingleChoice.ToQuestionTypeString(),
                TopicId = topicId,
                Answers = new List<AnswerEntity>
                {
                    new AnswerEntity
                    {
                         Id = Guid.NewGuid().ToStringWithoutHyphens(),
                        Text = "To invert control and supply dependencies from the outside",
                        IsCorrect = true
                    },
                    new AnswerEntity
                    {
                         Id = Guid.NewGuid().ToStringWithoutHyphens(),
                        Text = "To create dependencies inside the class using new",
                        IsCorrect = false
                    },
                    new AnswerEntity
                    {
                         Id = Guid.NewGuid().ToStringWithoutHyphens(),
                        Text = "To optimize for speed by avoiding interfaces",
                        IsCorrect = false
                    }
                }.SerializeOrDefault()
            },
            new QuestionEntity
            {
                ExternalId = "backend-di-q2",
                Text = "Which technique is commonly used in DI to provide dependencies through a constructor?",
                Difficulty = difficultyLevel,
                Type = TestQuestionType.SingleChoice.ToQuestionTypeString(),
                TopicId = topicId,
                Answers = new List<AnswerEntity>
                {
                    new AnswerEntity
                    {
                        Id = Guid.NewGuid().ToStringWithoutHyphens(),
                        Text = "Constructor injection",
                        IsCorrect = true
                    },
                    new AnswerEntity
                    {
                         Id = Guid.NewGuid().ToStringWithoutHyphens(),
                        Text = "Global singleton access",
                        IsCorrect = false
                    },
                    new AnswerEntity
                    {
                         Id = Guid.NewGuid().ToStringWithoutHyphens(),
                        Text = "Direct instantiation with new inside the consumer",
                        IsCorrect = false
                    }
                }.SerializeOrDefault()
            }
        };
        await _topicQuestionsRepository.InsertQuestionsAsync(topicId, questions, ct);
        return Ok();
    }

    [HttpGet("searchQuestions")]
    public async Task<IActionResult> SearchQuestions(CancellationToken ct)
    {
        var topicName = "dependency";
        var topicDescription = "inversion of Control by injecting dependencies rather than creating them directly.";
        var difficultyLevel = Difficulty.Medium.ToString();

        var topicEntity = new TopicEntity
        {
            ExternalId = "backend-di",
            Name = topicName,
            Description = topicDescription
        };

        // goal to find questions in db for this topic
        var similarTopic = await _topicQuestionsRepository.SearchTopicsAsync(
            searchId: topicEntity.ExternalId,
            searchName: topicEntity.Name,
            searchDescription: topicEntity.Description,
            ct);

        var questions = await _topicQuestionsRepository.GetQuestionsByTopicIdAsync(similarTopic.First().Id, difficultyLevel, ct);

        var questionsDto = questions.Select(q => new QuestionDto
        {
            Id = q.Id.ToString(),
            Text = q.Text,
            Type = q.Type.FromQuestionTypeString(),
            Answers = q.Answers.DeserializeOrDefault<List<AnswerDto>>() ?? new List<AnswerDto>()
        }).ToList();

        return Ok(questionsDto);
    }

    
//{
//      "id": null,
//      "text": "Which statement best describes dependency injection?",
//      "type": 0,
//      "answers": [
//        {
//          "id": null,
//          "text": "It injects dependencies from outside the class",
//          "isCorrect": true
//        },
//        {
//          "id": null,
//          "text": "It makes the class create its own dependencies",
//          "isCorrect": false
//        },
//        {
//    "id": null,
//          "text": "It prevents using interfaces",
//          "isCorrect": false
//        }
//      ]
//    },
//    {
//    "id": null,
//      "text": "What principle does dependency injection primarily promote?",
//      "type": 0,
//      "answers": [
//        {
//        "id": null,
//          "text": "Tight coupling",
//          "isCorrect": false
//        },
//        {
//        "id": null,
//          "text": "Inversion of Control",
//          "isCorrect": true
//        },
//        {
//        "id": null,
//          "text": "Manual dependency creation",
//          "isCorrect": false
//        }
//      ]
//    }
    [HttpPost("bulk")]
    public async Task<IActionResult> Bulk(CancellationToken ct)
    {
        var topicName = "Dependency Injection";
        var topicDescription = "A design pattern used to achieve Inversion of Control by injecting dependencies rather than creating them directly.";
        var difficultyLevel = Difficulty.Easy.ToString();

        var topicEntity = new TopicEntity
        {
            ExternalId = "backend-di",
            Name = topicName,
            Description = topicDescription
        };

        var questions = new List<QuestionDto>
        {
            new QuestionDto
            {
                Id = null,
                Text = "Which statement best describes dependency injection?",
                Type = TestQuestionType.SingleChoice,
                IsGenerated = true,
                Answers = new List<AnswerDto>
                {
                    new AnswerDto
                    {
                        Id = null,
                        Text = "It injects dependencies from outside the class",
                        IsCorrect = true
                    },
                    new AnswerDto
                    {
                        Id = null,
                        Text = "It makes the class create its own dependencies",
                        IsCorrect = false
                    },
                    new AnswerDto
                    {
                        Id = null,
                        Text = "It prevents using interfaces",
                        IsCorrect = false
                    }
                }
            },
            new QuestionDto
            {
                Id = null,
                Text = "What principle does dependency injection primarily promote?",
                Type = TestQuestionType.SingleChoice,
                IsGenerated = true,
                Answers = new List<AnswerDto>
                {
                    new AnswerDto
                    {
                        Id = null,
                        Text = "Tight coupling",
                        IsCorrect = false
                    },
                    new AnswerDto
                    {
                        Id = null,
                        Text = "Inversion of Control",
                        IsCorrect = true
                    },
                    new AnswerDto
                    {
                        Id = null,
                        Text = "Manual dependency creation",
                        IsCorrect = false
                    }
                }
            }
        };

        questions.ForEach(q =>
        {
            q.Id = Guid.NewGuid().ToStringWithoutHyphens();
            q.Answers.ForEach(a =>
            {
                a.Id = Guid.NewGuid().ToStringWithoutHyphens();
            });
        });
        var questionEntities = questions.Where(q => q.IsGenerated).Select(q => new Persistence.Models.QuestionEntity
        {
            ExternalId = q.Id,
            Text = q.Text,
            Difficulty = difficultyLevel,
            Type = q.Type.ToQuestionTypeString(),
            Answers = q.Answers.SerializeOrDefault(),
        });

        var topicId = await _topicQuestionsRepository.InsertTopicWithQuestions(topicEntity, questionEntities, ct);
        return Ok(topicId);
    }
}
