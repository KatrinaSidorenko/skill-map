using System.Text.Json.Serialization;

using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

namespace LearningPlatform.Workspace.WebSockets.Actions;

public class DeleteLearningItemAction : WorkspaceActionRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("incidentConnectionIds")]
    public List<string> IncidentConnectionIds { get; set; }

    [JsonPropertyName("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonPropertyName("idempotencyKey")]
    public string IdempotencyKey { get; set; }

    public override WorkspaceAction ToWorkspaceAction(string workspaceId)
        => new WorkspaceAction(long.Parse(workspaceId), WorkspaceActionType.DeleteLearningItem, ToCommand());

    protected override IWorkspaceActionCommand ToCommand()
        => new DeleteLearningItemActionCommand(Id, IncidentConnectionIds, ClientWorkspaceVersion, IdempotencyKey);
}