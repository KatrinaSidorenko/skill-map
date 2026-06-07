using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;

using QuestionDto = LearningPlatform.RoadmapTests.Service.Application.Models.QuestionDto;

namespace LearningPlatform.RoadmapTests.Service.Application.Abstractions;

public interface IQuestionSource
{
    Task<GenerationResult<List<QuestionDto>>> GetUniqueQuestionsForTopic(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct);
}