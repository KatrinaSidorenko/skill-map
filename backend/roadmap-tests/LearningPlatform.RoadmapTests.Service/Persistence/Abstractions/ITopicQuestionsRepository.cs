using LearningPlatform.RoadmapTests.Service.Persistence.Models;

namespace LearningPlatform.RoadmapTests.Service.Persistence.Abstractions;

public interface ITopicQuestionsRepository
{
    Task<IReadOnlyList<QuestionEntity>> GetQuestionsByTopicIdAsync(long topicId, string difficultyLevel, CancellationToken ct);
    Task<TopicEntity> GetTopicByExternalIdAsync(string externalId, CancellationToken ct);
    Task InsertQuestionsAsync(long topicId, IEnumerable<QuestionEntity> questions, CancellationToken ct);
    Task<long> InsertTopicAsync(TopicEntity topic, CancellationToken ct);
    Task<long> InsertTopicWithQuestions(TopicEntity topic, IEnumerable<QuestionEntity> questions, CancellationToken ct);
    Task<IReadOnlyList<QuestionEntity>> SearchQuestionsByTopicTextAsync(string searchText, CancellationToken ct);
    Task<List<TopicEntity>> SearchTopicsAsync(string searchId, string searchName, string searchDescription, CancellationToken ct);
}
