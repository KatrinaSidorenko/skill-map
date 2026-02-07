using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningPlatform.RoadmapTests.Service.Persistence.Models;

public sealed class QuestionEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    [Column("external_id")]
    public string ExternalId { get; set; } = default!;
    [Column("text")]
    public string Text { get; set; } = default!;
    [Column("difficulty")]
    public string Difficulty { get; set; } = default!;
    [Column("type")]
    public string Type { get; set; } = default!;
    [Column("answers", TypeName = "jsonb")]
    public string Answers { get; set; } = default!;
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    [Column("topic_id")]
    public long TopicId { get; set; }
}

public sealed class AnswerEntity
{
    public string Id { get; set; } = default!;
    public string Text { get; set; } = default!;
    public bool IsCorrect { get; set; }
}