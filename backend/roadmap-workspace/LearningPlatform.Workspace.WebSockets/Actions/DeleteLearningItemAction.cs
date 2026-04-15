using System.Text.Json.Serialization;

namespace LearningPlatform.Workspace.WebSockets.Contracts.Actions;

public class DeleteLearningItemAction : WorkspaceActionRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonPropertyName("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    public override WorkspaceAction ToWorkspaceAction(string workspaceId)
        => new WorkspaceAction(workspaceId, WorkspaceActionType.DeleteLearningItem, this);
}