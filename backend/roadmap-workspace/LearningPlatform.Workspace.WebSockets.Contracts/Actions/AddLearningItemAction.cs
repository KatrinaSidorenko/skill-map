using System.Text.Json.Serialization;

namespace LearningPlatform.Workspace.WebSockets.Contracts.Actions;

public class AddLearningItemAction : WorkspaceActionRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("baseVersion")]
    public int BaseVersion { get; set; }

    [JsonPropertyName("idempotencyKey")]
    public string IdempotencyKey { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }

    public override WorkspaceAction ToWorkspaceAction(string workspaceId)
        => new WorkspaceAction(workspaceId, WorkspaceActionType.CreateLearningItem, this);
}