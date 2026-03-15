using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SkillMap.Core.RoadmapsWorkspace.Events;
public class LearningItemDeletedEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }

    public LearningItemDeletedEvent(string id)
    {
        Id = id;
    }
}
