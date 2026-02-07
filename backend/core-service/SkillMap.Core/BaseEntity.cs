using System.ComponentModel.DataAnnotations;

namespace SkillMap.Core;

public abstract class BaseEntity
{
    [Key]
    public long Id { get; set; }
}