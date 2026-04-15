using System.Text.Json.Serialization;

using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

namespace LearningPlatform.Workspace.WebSockets.Actions;

public class UpdateLearningItemAction : WorkspaceActionRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("type")]
  public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("baseVersion")]
    public int BaseVersion { get; set; }

    [JsonPropertyName("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    public override WorkspaceAction ToWorkspaceAction(string workspaceId)
        => new WorkspaceAction(workspaceId, WorkspaceActionType.UpdateLearningItem, ToCommand());

protected override IWorkspaceActionCommand ToCommand()
        => new UpdateLearningItemActionCommand(Id, Title, Description, Status, Type, BaseVersion, IdempotencyKey);
}