using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkillMap.Core.Constants;

namespace SkillMap.Core.PersonalizedRoadmaps;

// todo: add json perops
public class RoadmapSnapshot
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public double Progress { get; set; }
    public DateTime SavedAt { get; set; }
    public string Status { get; set; }

    public List<LearningItemSnapshot> LearningItems { get; set; }
    public List<LearningItemsConnectionSnapshot> LearningItemsConnections { get; set; }

}

public record LearningItemSnapshot(string Id, string Title, string Description, LearningStatus Status);
public record LearningItemsConnectionSnapshot(string Id, string FromId, string ToId);