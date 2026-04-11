namespace SkillMap.Core.RoadmapsWorkspace;
public class RoadmapLearningProjection : TrackedEntity
{
    public long RoadmapWorkspaceId { get; private set; }
    public string LearningItemId { get; private set; }
    public bool IsAvailable { get; private set; }
    public string Status { get; private set; }

    public virtual RoadmapWorkspace RoadmapWorkspace { get; set; }

    public RoadmapLearningProjection(long roadmapWorkspaceId, string learningItemId, bool isAvailable, string status)
    {
        RoadmapWorkspaceId = roadmapWorkspaceId;
        LearningItemId = learningItemId;
        IsAvailable = isAvailable;
        Status = status;
    }

    public void UpdateStatus(string? newStatus)
    {
        Status = newStatus ?? Status;
    }

    public void UpdateAvailability(bool? isAvailable)
    {
        if (isAvailable.HasValue)
        {
            IsAvailable = isAvailable.Value;
        }
    }
}
