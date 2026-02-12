using System.Text;

using Dapper;

using LearningPlatform.RoadmapTests.Service.Persistence;
using LearningPlatform.RoadmapTests.Service.Persistence.Abstractions;
using LearningPlatform.RoadmapTests.Service.Persistence.Models;

public sealed class TopicQuestionsRepository : ITopicQuestionsRepository
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public TopicQuestionsRepository(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // todo: errors handling
    public async Task<long> InsertTopicAsync(
        TopicEntity topic,
        CancellationToken ct)
    {
        const string sql = """
            INSERT INTO roadmap_test.topics (external_id, name, description)
            VALUES (@ExternalId, @Name, @Description)
            RETURNING id;
            """;

        using var conn =
            await _connectionFactory.CreateOpenConnectionAsync(ct);

        return await conn.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, topic, cancellationToken: ct));
    }

    public async Task InsertQuestionsAsync(
        long topicId,
        IEnumerable<QuestionEntity> questions,
        CancellationToken ct)
    {
        const string sql = """
            INSERT INTO roadmap_test.questions
                (external_id, text, difficulty, type, answers, topic_id)
            VALUES
                (@ExternalId, @Text, @Difficulty, @Type, @Answers::jsonb, @TopicId)
            """;

        using var conn =
            await _connectionFactory.CreateOpenConnectionAsync(ct);

        using var tx = conn.BeginTransaction();

        foreach (var question in questions)
        {
            await conn.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    question,
                    transaction: tx,
                    cancellationToken: ct));
        }

        tx.Commit();
    }


    public async Task<TopicEntity?> GetTopicByExternalIdAsync(
        string externalId,
        CancellationToken ct)
    {
        const string sql = """
            SELECT *
            FROM roadmap_test.topics
            WHERE external_id = @externalId
            LIMIT 1;
            """;

        using var conn =
            await _connectionFactory.CreateOpenConnectionAsync(ct);

        return await conn.QuerySingleOrDefaultAsync<TopicEntity>(
            new CommandDefinition(
                sql,
                new { externalId },
                cancellationToken: ct));
    }

    public async Task<IReadOnlyList<QuestionEntity>> GetQuestionsByTopicIdAsync(
        long topicId,
        string difficultyLevel,
        CancellationToken ct)
    {
        const string sql = """
            SELECT q.*
            FROM roadmap_test.questions q
            WHERE q.topic_id = @topicId AND q.difficulty = @difficultyLevel
            ORDER BY q.id;
""";

        using var conn =
            await _connectionFactory.CreateOpenConnectionAsync(ct);

        var result = await conn.QueryAsync<QuestionEntity>(
            new CommandDefinition(
                sql,
                new { topicId, difficultyLevel },
                cancellationToken: ct));

        return result.AsList();
    }

    // add dificulty level
    public async Task<IReadOnlyList<QuestionEntity>> SearchQuestionsByTopicTextAsync(
        string searchText,
        CancellationToken ct)
    {
        const string sql = """
            SELECT q.*
            FROM roadmap_test.questions q
            JOIN roadmap_test.topics t ON q.external_id LIKE t.external_id || '%'
            WHERE
                t.name ILIKE '%' || @searchText || '%'
                OR t.description ILIKE '%' || @searchText || '%'
                OR q.text ILIKE '%' || @searchText || '%'
            ORDER BY q.created_at DESC
            LIMIT 50;
            """;

        using var conn =
            await _connectionFactory.CreateOpenConnectionAsync(ct);

        var result = await conn.QueryAsync<QuestionEntity>(
            new CommandDefinition(
                sql,
                new { searchText },
                cancellationToken: ct));

        return result.AsList();
    }

    public async Task<List<TopicEntity>> SearchTopicsAsync(
        string searchId,
        string searchName,
        string searchDescription,
        CancellationToken ct)
    {

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(searchId))
        {
            conditions.Add("external_id ILIKE '%' || @searchId || '%'");
            parameters.Add("searchId", searchId);
        }

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            conditions.Add("name ILIKE '%' || @searchName || '%'");
            parameters.Add("searchName", searchName);
        }

        if (!string.IsNullOrWhiteSpace(searchDescription))
        {
            conditions.Add("description ILIKE '%' || @searchDescription || '%'");
            parameters.Add("searchDescription", searchDescription);
        }

        var sql = new StringBuilder("SELECT * FROM roadmap_test.topics ");

        if (conditions.Count > 0)
        {
            sql.AppendLine("WHERE " + string.Join(" AND ", conditions));
        }

        sql.AppendLine(" ORDER BY created_at DESC LIMIT 50");


        using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);

        var result = await conn.QueryAsync<TopicEntity>(
            new CommandDefinition(
                sql.ToString(),
                parameters,
                cancellationToken: ct));

        return result.AsList();

    }

    public async Task<long> InsertTopicWithQuestions(
        TopicEntity topic,
        IEnumerable<QuestionEntity> questions,
        CancellationToken ct)
    {
        const string sqlInsertTopic = """
            INSERT INTO roadmap_test.topics (external_id, name, description)
            VALUES (@ExternalId, @Name, @Description)
            RETURNING id;
            """;
        const string sqlTopicExists = """
            SELECT id
            FROM roadmap_test.topics
            WHERE external_id = @ExternalId or name LIKE @Name or description LIKE @Description
            LIMIT 1;
            """;

        const string sqlInsertQuestion = """
            INSERT INTO roadmap_test.questions
                (external_id, text, difficulty, type, answers, topic_id)
            VALUES
                (@ExternalId, @Text, @Difficulty, @Type, @Answers::jsonb, @TopicId)
            """;

        using var conn = await _connectionFactory.CreateOpenConnectionAsync(ct);
        using var tx = conn.BeginTransaction();

        var topicId = await conn.ExecuteScalarAsync<long?>(
            new CommandDefinition(
                sqlTopicExists,
                topic,
                transaction: tx,
                cancellationToken: ct));
        if (!topicId.HasValue)
        {
            topicId = await conn.ExecuteScalarAsync<long>(new CommandDefinition(sqlInsertTopic, topic, transaction: tx, cancellationToken: ct));
        }
        if (!topicId.HasValue)
        {
            throw new InvalidOperationException("Failed to insert or retrieve topic ID.");
        }

        foreach (var question in questions)
        {
            question.TopicId = topicId.Value;
            await conn.ExecuteAsync(
                new CommandDefinition(
                    sqlInsertQuestion,
                    question,
                    transaction: tx,
                    cancellationToken: ct));
        }
        tx.Commit();
        return topicId.Value;
    }
}