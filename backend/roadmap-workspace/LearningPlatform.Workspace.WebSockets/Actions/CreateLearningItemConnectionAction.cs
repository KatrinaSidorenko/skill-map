using System.Text.Json.Serialization;

using LearningPlatform.Workspace.WebSockets.Contracts;
using LearningPlatform.Workspace.WebSockets.Contracts.Commands;

namespace LearningPlatform.Workspace.WebSockets.Actions;

public class CreateLearningItemConnectionAction : WorkspaceActionRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("target")]
    public string Target { get; set; }

    [JsonPropertyName("clientWorkspaceVersion")]
    public int ClientWorkspaceVersion { get; set; }

    [JsonPropertyName("idempotencyKey")]
    public string IdempotencyKey { get; set; }
    public override WorkspaceAction ToWorkspaceAction(string workspaceId)
        => new WorkspaceAction(long.Parse(workspaceId), WorkspaceActionType.CreateConnection, ToCommand());

    protected override IWorkspaceActionCommand ToCommand()
        => new CreateLearningItemConnectionActionCommand(Id, Source, Target, ClientWorkspaceVersion, IdempotencyKey);
}