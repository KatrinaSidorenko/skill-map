using Newtonsoft.Json;

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

public class InboxTaskOutput
{
    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; }
    [JsonProperty("result")]
    public string? Result { get; set; }
    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }
}

public class InboxTask : TrackedEntity
{
    public string Input { get; set; }
    public TaskType TaskType { get; set; }
    public TaskStatus Status { get; set; }
    public Guid? WorkerId { get; set; }
    public string? Output { get; set; }

    public InboxTask() { }
    public InboxTask(string input, TaskType taskType)
    {
        Input = input;
        TaskType = taskType;
        Status = TaskStatus.Pending;
        WorkerId = null;
        Output = null;
    }
}
