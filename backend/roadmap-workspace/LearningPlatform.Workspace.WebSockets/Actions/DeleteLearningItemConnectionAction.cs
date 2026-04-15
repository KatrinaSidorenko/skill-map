using System.Text.Json.Serialization;

using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

namespace LearningPlatform.Workspace.WebSockets.Actions;

public class DeleteLearningItemConnectionAction : WorkspaceActionRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonPropertyName("idempotencyKey")]
    public string IdempotencyKey { get; set; }
    [JsonIgnore]
    private long WorkspaceIdLong => long.Parse(Id);
    public override WorkspaceAction ToWorkspaceAction(string workspaceId)
        => new WorkspaceAction(WorkspaceIdLong, WorkspaceActionType.DeleteConnection, ToCommand());

    protected override IWorkspaceActionCommand ToCommand()
        => new DeleteLearningItemConnectionActionCommand(Id, ClientWorkspaceVersion, IdempotencyKey);
}