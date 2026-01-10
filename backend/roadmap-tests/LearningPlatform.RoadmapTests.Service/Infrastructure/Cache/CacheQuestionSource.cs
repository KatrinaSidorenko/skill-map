using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;
using LearningPlatform.Shared.Caching.Abstractions;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.Cache;

public class CacheQuestionSource(ICacheService cacheService) : ICacheQuestionSource
{
    public async Task<GenerationResult<List<Application.Models.QuestionDto>>> Generate(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        return new GenerationResult<List<Application.Models.QuestionDto>>([]);
    }
}
