using LearningPlatform.RoadmapTests.Contracts.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.Models;
using LearningPlatform.RoadmapTests.Service.TopicQuestion.QuestionsGenerator;
using TopicQuestionsDto = LearningPlatform.RoadmapTests.Service.TopicQuestion.Models.TopicQuestionsDto;

namespace LearningPlatform.RoadmapTests.Service.TopicQuestion;

public sealed class TopicQuestionGenerationService
: ITopicQuestionGenerationService
{
    private readonly IQuestionGenerator _generator;

    public TopicQuestionGenerationService(
        IQuestionGenerator generator)
    {
        _generator = generator;
    }

    public async Task<TopicQuestionsDto> GenerateTopicQuestions(
        TopicDto topic,
        TopicQuestionsSettingDto settings,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (settings.QuestionsCount <= 0)
            throw new ArgumentException("QuestionsCount must be > 0");

        if (settings.Types.Count == 0)
            throw new ArgumentException("At least one question type required");

        var questions = await _generator.Generate(topic, settings, ct);

        return new TopicQuestionsDto
        {
            Id = topic.Id,
            Questions = questions
        };
    }
}
