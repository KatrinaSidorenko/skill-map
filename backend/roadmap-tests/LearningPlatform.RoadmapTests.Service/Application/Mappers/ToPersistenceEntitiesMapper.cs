namespace LearningPlatform.RoadmapTests.Service.Application.Mappers;

using LearningPlatform.RoadmapTests.Service.Application.Models;
using LearningPlatform.RoadmapTests.Service.Persistence.Models;

using Newtonsoft.Json;

public static class ToPersistenceEntitiesMapper
{
    // ---------------------------
    // TOPIC
    // ---------------------------
    public static TopicEntity ToTopicEntity(
        this TopicQuestionsDto dto,
        string topicName,
        string? topicDescription)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentException("Topic external id is required");

        return new TopicEntity
        {
            ExternalId = dto.Id,
            Name = topicName,
            Description = topicDescription,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    // ---------------------------
    // QUESTIONS
    // ---------------------------
    public static IReadOnlyList<QuestionEntity> ToQuestionEntities(
        this TopicQuestionsDto dto,
        string difficulty)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        return dto.Questions.Select(q => ToQuestionEntity(q, difficulty))
                            .ToList();
    }

    private static QuestionEntity ToQuestionEntity(
        QuestionDto dto,
        string difficulty)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentException("Question external id is required");

        return new QuestionEntity
        {
            ExternalId = dto.Id,
            Text = dto.Text,
            Difficulty = difficulty,
            Type = dto.Type.ToString(),
            Answers = SerializeAnswers(dto.Answers),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    // ---------------------------
    // ANSWERS SERIALIZATION
    // ---------------------------
    private static string SerializeAnswers(List<AnswerDto> answers)
    {
        if (answers is null || answers.Count == 0)
            throw new ArgumentException("Question must contain answers");

        // store a minimal, stable JSON structure
        var payload = answers.Select(a => new
        {
            id = a.Id,
            text = a.Text,
            isCorrect = a.IsCorrect
        });

        return JsonConvert.SerializeObject(payload);
    }
}