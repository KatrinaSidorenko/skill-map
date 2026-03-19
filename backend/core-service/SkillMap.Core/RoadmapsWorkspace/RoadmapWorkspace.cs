using Newtonsoft.Json;

using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.Roadmaps;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Core.User;

namespace SkillMap.Core.RoadmapsWorkspace;

public class RoadmapWorkspaceMetadata
{
    [JsonProperty("title")]
    public string Title { get; private set; }
    [JsonProperty("description")]
    public string Description { get; private set; }
    [JsonProperty("imageUrl")]
    public string ImageUrl { get; private set; }
    public RoadmapWorkspaceMetadata(string title, string description, string imageUrl)
    {
        Title = title;
        Description = description;
        ImageUrl = imageUrl;
    }
}

public class RoadmapWorkspace : TrackedEntity
{
    public long AuthorId { get; private set; }
    public string? RoadmapId { get; private set; }
    public long? PersonalRoadmapId { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsInAuthorMode { get; private set; }
    public string MetadataJson { get; private set; }

    public virtual AppUser Author { get; set; }
    public virtual PersonalRoadmap? PersonalRoadmap { get; set; }
    public virtual ICollection<RoadmapWorkspaceEvent> WorkspaceEvents { get; set; }
    public virtual ICollection<RoadmapWorkspaceSnapshot> Snapshots { get; set; }
    public virtual ICollection<RoadmapAssessment> Assessments { get; set; }


    public RoadmapWorkspace() { }
    public RoadmapWorkspace(long userId, string? roadmapId, long? personalRoadmapId)
    {
        AuthorId = userId;
        RoadmapId = roadmapId;
        PersonalRoadmapId = personalRoadmapId;
        IsActive = true;
        IsInAuthorMode = false;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ActivateAuthorMode()
    {
        IsInAuthorMode = true;
    }
    public void SetMetadata(RoadmapWorkspaceMetadata metadata)
    {
        MetadataJson = JsonConvert.SerializeObject(metadata);
    }
    public string ActualRoadmapId => IsInAuthorMode ? PersonalRoadmapId.ToString() ?? throw new ArgumentException("Something went wrong with roadmap id") : RoadmapId;
    public RoadmapWorkspaceMetadata Metadata => JsonConvert.DeserializeObject<RoadmapWorkspaceMetadata>(MetadataJson) ?? throw new ArgumentException("Something went wrong with roadmap workspace metadata");
}