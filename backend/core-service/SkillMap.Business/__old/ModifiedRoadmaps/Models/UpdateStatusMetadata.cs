using Newtonsoft.Json;

using SkillMap.Core.Constants;

namespace SkillMap.Business.__old.ModifiedRoadmaps.Models;

public class UpdateStatusMetadata
{
    [JsonProperty("status")]
    public LearningStatus Status { get; set; }
}