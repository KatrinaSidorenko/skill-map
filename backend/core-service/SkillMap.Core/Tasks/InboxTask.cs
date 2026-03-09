namespace SkillMap.Core.Tasks;

public enum TaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Error = 3,
}

public enum TaskType
{
    BuildWorkspaceSnapshot = 0,
}

public class InboxTask : TrackedEntity
{
    public string Input { get; set; }
    public TaskType TaskType { get; set; }
    public TaskStatus Status { get; set; }
    public long? WorkerId { get; set; }
    public string? Output { get; set; }
}
