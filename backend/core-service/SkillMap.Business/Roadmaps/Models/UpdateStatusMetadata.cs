using Newtonsoft.Json;
using SkillMap.Core.Constants;

namespace SkillMap.Business.Roadmaps.Models;

public class UpdateStatusMetadata
{
    [JsonProperty("status")]
    public LearningStatus Status { get; set; }
}
