using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SkillMap.Business.RoadmapTest.Models;

public class RoadmapChangesSuggestionsDto
{
    [JsonProperty("suggestions")]
    public List<RoadmapTestSuggestionItemDto> Suggestions { get; set; } = new();
}

public class RoadmapTestSuggestionItemDto
{
    [JsonProperty("learningItemId")]
    public string LearningItemId { get; set; } = string.Empty;
    [JsonProperty("status")]
    public string LearningStatus { get; set; } = string.Empty;
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}