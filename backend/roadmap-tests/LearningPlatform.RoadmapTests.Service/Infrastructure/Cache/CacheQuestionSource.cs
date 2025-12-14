using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.Application.Abstractions;
using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Infrastructure.Common;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.Cache;

public class CacheQuestionSource : ICacheQuestionSource
{
    public async Task<GenerationResult<List<Application.Models.QuestionDto>>> Generate(TopicDto topic, TopicQuestionsSettingDto settings, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
