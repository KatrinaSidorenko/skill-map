using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningPlatform.RoadmapTests.Service.Persistence.Models
{
    public sealed class TopicEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("external_id")]
        public string ExternalId { get; set; } = default!;
        [Column("name")]
        public string Name { get; set; } = default!;
        [Column("description")]
        public string? Description { get; set; }
        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}